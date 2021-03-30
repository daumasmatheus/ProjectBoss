import { ProjectModel } from "./project.model";
import { TaskModel } from "./task.model";

export class UserDataModel {
    id: string;
    personId: string;
    provider: string;
    createdDate: Date;
    userName: string;
    email: string;
    role: UserRole;

    person: UserPersonModel;
}

export class UserPersonModel {
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
    projects: ProjectModel[];
}

export class UserRole{
    id: string;
    name: string;
}