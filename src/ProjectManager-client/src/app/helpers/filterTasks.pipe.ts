import { NumberInput } from '@angular/cdk/coercion';
import { Pipe, PipeTransform } from '@angular/core';
import { Tasks } from 'ngx-ui-loader';
import { TaskModel } from '../components/dashboard/models/task.model';

@Pipe({
    name: 'filterTask'
})

export class FilterTasksPipe implements PipeTransform {
    transform(value: TaskModel[], statusId: number): any {
        if (!value || !statusId)
            return value;

        return value.filter(x => x.statusId == statusId);
    }
}