import { ProjectTask } from './project-task.model';

export interface NewProject {
    authorId: string;

    title: string;
    startDate: Date;
    conclusionDate: Date;    
    description: string;

    tasks: ProjectTask[];
    attendantIds: string[];
}