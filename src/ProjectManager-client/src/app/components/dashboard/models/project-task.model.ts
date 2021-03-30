import { PersonModel } from "./person.1.model";

export class ProjectTask {
    constructor(_id: number) {
        this.id = _id;
    }

    id: number;
    taskId: string;
    projectId: string;
    authorId: string;
    attendantId: string;
    statusId: number;
    priorityId: number;
    title: string;
    description: string;
    conclusionDate: Date;    
    
    attendant: PersonModel;
}