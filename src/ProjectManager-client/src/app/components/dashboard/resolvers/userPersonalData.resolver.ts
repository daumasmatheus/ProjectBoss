import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { LocalStorageUtils } from 'src/app/helpers/localstorage';
import { PersonModel } from '../models/person.1.model';
import { PersonServices } from '../services/person.services';

@Injectable({ providedIn: 'root' })
export class UserPersonalDataResolver implements Resolve<PersonModel> {
    private localStorageUtils = new LocalStorageUtils();

    constructor (private personService: PersonServices) {}

    resolve(route: ActivatedRouteSnapshot): Observable<PersonModel> | Promise<PersonModel> | PersonModel {
        let personId = this.localStorageUtils.getUser().personId;

        return this.personService.getPersonData(personId).toPromise().then(
            data => {
                return data;
            }
        )
    }
}