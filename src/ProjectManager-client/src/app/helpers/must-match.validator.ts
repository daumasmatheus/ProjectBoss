import { FormGroup } from '@angular/forms';

export function MustMatch(controlName: string, matchingControlName: string){
    return (formGroup: FormGroup) => {
        const CONTROL = formGroup.controls[controlName];
        const MATCHING_CONTROL = formGroup.controls[matchingControlName];

        if (MATCHING_CONTROL.errors && !MATCHING_CONTROL.errors.mustMatch){
            return;
        }

        if (CONTROL.value !== MATCHING_CONTROL.value) {
            MATCHING_CONTROL.setErrors({ mustMatch: true });
        } else {
            MATCHING_CONTROL.setErrors(null);
        }
    }
}