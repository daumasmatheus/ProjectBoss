import { TaskModel } from "./task.model";

export class PersonModel {
    personId: string;
    personCode: number;
    userId: string;
    firstName: string;
    lastName: string;  
    fullName: string;  
    role: string;
    company: string;
    country: string;  
    tasksAsigned: number;  

    tasks: TaskModel[];
}