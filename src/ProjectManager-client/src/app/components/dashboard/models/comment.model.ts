import { PersonModel } from "./person.1.model";

export class CommentModel {
    content: string;
    personId: string;
    taskId: string;
    createdDate: Date;

    person: PersonModel;
}