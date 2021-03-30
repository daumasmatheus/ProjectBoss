import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { LocalStorageUtils } from 'src/app/helpers/localstorage';
import { PersonInProjectSimpleModel } from '../models/personInProject.model';
import { PersonInProjectServices } from '../services/personInProject.services';
import { ProjectServices } from '../services/project.services';

@Injectable({ providedIn: 'root' })
export class GetProjectIdsByPersonResolver implements Resolve<PersonInProjectSimpleModel[]> {
    localStorageUtils = new LocalStorageUtils();

    constructor (private personInProjectServices: PersonInProjectServices,
                 private projectServices: ProjectServices) { }

    resolve(route: ActivatedRouteSnapshot): Observable<PersonInProjectSimpleModel[]> | Promise<PersonInProjectSimpleModel[]> | PersonInProjectSimpleModel[] {
        let personId = this.localStorageUtils.getUser().personId;

        let isProjectManagerOrAdmin = this.localStorageUtils.checkUserClaim("ProjectManager") || this.localStorageUtils.checkUserClaim("Administrator");

        if (isProjectManagerOrAdmin) {
            return this.projectServices.getAllProjectsDataForDropdown().toPromise().then(
                (resp: PersonInProjectSimpleModel[]) => {
                    return resp;
                }
            );
        } else {
            return this.personInProjectServices.getAssignedProjects(personId).toPromise().then(
                (resp: PersonInProjectSimpleModel[]) => {
                    return resp;
                }
            );
        }        
    }
}