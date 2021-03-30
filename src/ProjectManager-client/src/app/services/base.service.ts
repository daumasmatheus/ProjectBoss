import { HttpErrorResponse, HttpHeaders, HttpResponse } from '@angular/common/http';
import { throwError } from 'rxjs';

import { LocalStorageUtils } from '../helpers/localstorage';

export abstract class BaseService {
    localStorage = new LocalStorageUtils();

    protected GetJsonHeader (){
        return {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        };
    }

    protected GetJsonAuthHeader() {
        return {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${this.localStorage.getUserToken()}`
            })
        };
    }

    protected GetAuthHeader() {
        return new HttpHeaders({
                'Authorization': `Bearer ${this.localStorage.getUserToken()}`
            });
    }

    protected ExtractData(response: any){
        return response.value || {};
    }

    protected ServiceError(response: Response | any){
        let customError: string[] = [];
        let customResponse = { error: { errors: [] }};

        if (response instanceof HttpErrorResponse) {
            if (response.statusText === "Unknown Error") {
                customError.push("Ocorreu um erro desconhecido");
                response.error.errors = customError;
            }
        }

        if(response.status === 500) {
            customError.push("Ocorreu um erro de processamento, tente novamente mais tarde ou contate o administrador do sistema");
            customResponse.error.errors = customError;

            return throwError(customResponse);
        }

        console.error(response);
        return throwError(response);
    }

}