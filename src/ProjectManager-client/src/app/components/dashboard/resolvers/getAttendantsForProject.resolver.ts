import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { PersonModel } from '../models/person.1.model';
import { PersonServices } from '../services/person.services';

@Injectable({ providedIn: 'root' })
export class GetAttendantsForProjectResolver implements Resolve<PersonModel[]> {

    constructor(private personServices: PersonServices) { }

    resolve(route: ActivatedRouteSnapshot): Observable<PersonModel[]> | Promise<PersonModel[]> | PersonModel[] {
        return this.personServices.getAll().toPromise().then(
            (resp: PersonModel[]) => {
                return resp;
            }
        )
    }
}