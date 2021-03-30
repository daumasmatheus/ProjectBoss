import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { LocalStorageUtils } from '../helpers/localstorage';

export abstract class BaseGuard {
    protected localStorageUtils = new LocalStorageUtils();

    constructor(protected router: Router) { }

    protected ValidateClaims(routeAc: ActivatedRouteSnapshot) {
        if (!this.localStorageUtils.getUserToken()) {
            this.router.navigate(['/account/login']);
        }

        let user = this.localStorageUtils.getUser();
        let claim: any = routeAc.data[0];

        if (claim !== undefined) {
            let claim = routeAc.data[0]['claim'];

            if (claim) {
                if (!user.claims) {
                    this.navigateToAccessDenied();
                }

                let userClaims = user.claims.find(x => x.type === claim.nome);

                if (!userClaims){
                    this.navigateToAccessDenied();
                }

                let claimValues = userClaims.value as string;

                if (!claimValues.includes(claim.valor)){
                    this.navigateToAccessDenied();
                }
            }
        }
    }

    protected navigateToAccessDenied() {
        this.router.navigate(['/access-denied']);
    }

    protected navigateToLogin() {
        this.router.navigate(['/account/login']);
    }

    protected navigateToDashboard() {
        this.router.navigate(['/dashboard/overview']);
    }
}