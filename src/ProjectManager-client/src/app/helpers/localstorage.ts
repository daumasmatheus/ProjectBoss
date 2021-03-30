export class LocalStorageUtils {

    getUser() {
        return JSON.parse(localStorage.getItem('pj.user'));
    }

    saveUserData(response: any) {
        this.saveUserToken(response.accessToken);
        this.saveUser(response.userToken);
    }

    clearLocalUserData() {
        localStorage.removeItem('pj.token');
        localStorage.removeItem('pj.user');
    }

    saveUserToken(token: string) {
        localStorage.setItem('pj.token', token);
    }

    getUserToken() {
        return localStorage.getItem('pj.token');
    }

    saveUser(user: string) {
        localStorage.setItem('pj.user', JSON.stringify(user));
    }

    checkUserClaim(value: string) {
        let claimsArr = this.getUser().claims;
        return claimsArr.filter(x => x.value.toUpperCase() == value.toUpperCase()).length > 0;
    }
}