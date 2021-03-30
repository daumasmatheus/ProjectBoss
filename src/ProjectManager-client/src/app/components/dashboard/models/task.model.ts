import { Person } from "./person.model";
import { ProjectModel } from "./project.model";

export class TaskModel {
    taskId: string;
    projectId: string;    
    statusId: number;
    priorityId: number;
    title: string;
    description: string;
    createdDate: Date;
    updatedDate: Date;
    conclusionDate: Date;
    concludedDate: Date;    
    attendant: Person;  
    author: Person;  
    isComplete: boolean;

    project: ProjectModel
}