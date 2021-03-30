import { PersonModel } from "./person.1.model";
import { PersonInProjectModel } from "./personInProject.model";
import { TaskModel } from "./task.model";

export class ProjectModel {
    projectId: string;
    authorId: string;
    title: string;
    description: string;
    createdDate: Date;
    startDate: Date;
    conclusionDate: Date;
    concludedDate: Date;

    author: PersonModel;
    tasks: TaskModel[];
}

export class ProjectDataModel {
    projectId: string;
    authorId: string;
    title: string;
    description: string;
    createdDate: Date;
    startDate: Date;
    conclusionDate: Date;
    concludedDate: Date;
      
    author: PersonModel;
    personInProject: PersonInProjectModel[];
}