import { ProjectModel } from "./project.model";
import { TaskModel } from "./task.model";

export class PersonOverviewModel {
    concludedTasks: TaskModel[];
    tasksDueSoon: TaskModel[];
    recentProjects: ProjectModel[];
}