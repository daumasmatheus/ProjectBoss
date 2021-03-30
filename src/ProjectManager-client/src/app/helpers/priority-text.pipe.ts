import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'priorityText'
})

export class PriorityTextPipe implements PipeTransform {
    transform(value: number, ...args: any[]): any {
        switch (value) {
            case 1:
                return 'Baixa';
            case 2:
                return 'Normal';
            case 3:
                return 'Alta';            
        };
    }
}