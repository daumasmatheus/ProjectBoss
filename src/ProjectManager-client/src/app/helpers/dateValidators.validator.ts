import { FormControl, FormGroup } from '@angular/forms';
import * as moment from 'moment';

export function DateCantBeLowerThanToday(control: FormControl) {
    if (control.value) {
        let date = moment(control.value);
        let today = moment();

        if (date.isBefore(today, 'day')) {
            return { 'invalidDate': true }; 
        }
    }

    return null;
}

export const TaskConclusionDateCantBeHigherThanProjectConclusionDate = (projectConclusionDate: Date) => {
    return (control: FormControl) => {
        let taskConclusionDate = moment(control.value);
        let projConclusionDate = moment(projectConclusionDate);

        if (taskConclusionDate.isAfter(projConclusionDate, 'day')) {
            return { 'invalidTaskConclusionDate': true };
        }
        return null;
    }
}