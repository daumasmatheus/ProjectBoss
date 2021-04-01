import { AvatarModule } from 'ngx-avatar';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FaIconLibrary, FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faCheckCircle, faClone, faComment} from "@fortawesome/free-regular-svg-icons";
import { NgxChartsModule } from "@swimlane/ngx-charts";
import { faArrowLeft, faCheck, faEdit, faEllipsisV, faExternalLinkAlt, faFileDownload, faFileExcel, faFileImport, 
         faFilePdf, faHome, faPause, faPlay, faPlus, faProjectDiagram, faQuestion, faQuestionCircle, faSignOutAlt, faTasks, faTrash, faUndo, faUserCircle, faUserPlus, faUsers, faUserSlash } from '@fortawesome/free-solid-svg-icons';

import { DashboardRoutingModule } from './dashboard.route';
import { DashboardComponent } from './dashboard.app.component';
import { AngularMaterialModule } from 'src/app/angular-material.module';
import { NavigationModule } from '../navigation/navigation.module';
import { OverviewComponent } from './overview/overview.component';
import { JwtValidationGuard } from './guards/jwt-validation.guard';
import { ProfileComponent } from './profile/profile.component';
import { ProjectComponent } from './projects/project/project.component';
import { ProjectHeaderComponent } from './projects/project-header/project-header.component';
import { NewProjectComponent } from './projects/new-project/new-project.component';
import { TableComponentComponent } from './base-components/table-component/table-component.component';
import { AddTaskDialogComponent } from './projects/add-task-dialog/add-task-dialog.component';
import { MomentPipe } from 'src/app/helpers/moment.pipe';
import { TasksComponent } from './tasks/tasks.component';
import { PriorityTextPipe } from 'src/app/helpers/priority-text.pipe';
import { AddNewTaskComponent } from './tasks/add-new-task/add-new-task.component';
import { TaskServices } from './services/task.services';
import { PersonServices } from './services/person.services';
import { UserPersonalDataResolver } from './resolvers/userPersonalData.resolver';
import { GetAttendantsForProjectResolver } from './resolvers/getAttendantsForProject.resolver';
import { ProjectServices } from "./services/project.services";
import { PersonInProjectServices } from './services/personInProject.services';
import { BoardComponent } from './projects/board/board.component';
import { FilterTasksPipe } from 'src/app/helpers/filterTasks.pipe';
import { ProjectDetailComponent } from './projects/project-detail/project-detail.component';
import { AttendantsComponent } from './projects/attendants/attendants.component';
import { SelectAttendantDialogComponent } from './projects/attendants/select-attendant-dialog/select-attendant-dialog.component';
import { CommentServices } from './services/comment.services';
import { StatisticsComponent } from './statistics/statistics.component';
import { StatisticsServices } from './services/statistics.services';
import { NewProjectGuard } from './guards/new-project.guard';
import { AccountService } from '../account/services/account.service';
import { ChartsModule } from 'ng2-charts';
import { SelectAttendantListComponent } from './base-components/select-attendant-list/select-attendant-list.component';
import { GetProjectIdsByPersonResolver } from './resolvers/getProjectIdsByPerson.resolver';
import { ArraySortPipe } from 'src/app/helpers/ArraySortPipe.pipe';
import { ManageUsersComponent } from './admin/manage-users/manage-users.component';
import { ManageUsersGuard } from './guards/manage-users.guard';
import { UsersServices } from './services/users.services';
import { UserDetailsComponent } from './admin/user-details/user-details.component';

@NgModule({
    declarations: [
        DashboardComponent,
        OverviewComponent,
        ProfileComponent,
        ProjectComponent,
        ProjectHeaderComponent,
        NewProjectComponent,
        TableComponentComponent,
        AddTaskDialogComponent,
        MomentPipe,
        PriorityTextPipe,
        FilterTasksPipe,
        ArraySortPipe,
        TasksComponent,
        AddNewTaskComponent,
        BoardComponent,
        ProjectDetailComponent,
        AttendantsComponent,        
        SelectAttendantDialogComponent, 
        StatisticsComponent, 
        SelectAttendantListComponent, 
        ManageUsersComponent, UserDetailsComponent        
    ],
    imports: [ 
        CommonModule,
        DashboardRoutingModule,
        AngularMaterialModule,
        FlexLayoutModule,
        NavigationModule,
        HttpClientModule,
        FormsModule,
        ReactiveFormsModule,
        AvatarModule,
        FontAwesomeModule,
        NgxChartsModule,
        ChartsModule
    ],
    exports: [],
    providers: [
        JwtValidationGuard,
        NewProjectGuard,
        ManageUsersGuard,
        UserPersonalDataResolver,
        GetAttendantsForProjectResolver,
        GetProjectIdsByPersonResolver,
        TaskServices,
        PersonServices,
        ProjectServices,
        PersonInProjectServices,
        CommentServices,
        StatisticsServices,
        AccountService,
        UsersServices
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class DashboardModule { 
    constructor(library: FaIconLibrary) {
        library.addIcons(faTasks, faProjectDiagram, faSignOutAlt, faUserCircle, faPlus, faFileImport, faFilePdf, 
                         faFilePdf, faFileExcel, faFileExcel, faHome, faEllipsisV, faCheck, faCheckCircle, faExternalLinkAlt, 
                         faEdit, faEdit, faPlay, faPause, faTrash, faClone, faFileDownload, faArrowLeft, faEdit, 
                         faUserSlash, faUserPlus, faComment, faUsers, faPlus, faQuestion, faUndo);        
    }    
}