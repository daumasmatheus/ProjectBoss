import { ProjectModel } from "./project.model";

export class PersonInProjectModel {
    personId: string;
    projectId: string;

    project: ProjectModel;
}

export class PersonInProjectSimpleModel {
    personId: string;
    projectId: string;
    projectName: string;
}