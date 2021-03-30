import { Routes, RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';

import { DashboardComponent } from './dashboard.app.component';
import { OverviewComponent } from './overview/overview.component';
import { JwtValidationGuard } from './guards/jwt-validation.guard';
import { ProjectComponent } from './projects/project/project.component';
import { NewProjectComponent } from './projects/new-project/new-project.component';
import { TasksComponent } from './tasks/tasks.component';
import { UserPersonalDataResolver } from './resolvers/userPersonalData.resolver';
import { GetAttendantsForProjectResolver } from './resolvers/getAttendantsForProject.resolver';
import { NewProjectGuard } from './guards/new-project.guard';
import { GetProjectIdsByPersonResolver } from './resolvers/getProjectIdsByPerson.resolver';
import { ManageUsersComponent } from './admin/manage-users/manage-users.component';
import { ManageUsersGuard } from './guards/manage-users.guard';

const routes: Routes = [
    {
        path: '', component: DashboardComponent, canActivate: [JwtValidationGuard], resolve: { personData: UserPersonalDataResolver },
        children: [     
            { path: '', pathMatch: 'full', redirectTo: 'overview'},       
            { path: 'overview', component: OverviewComponent },          
            { path: 'project', component: ProjectComponent, resolve: { projects: GetProjectIdsByPersonResolver } },          
            { path: 'tasks', component: TasksComponent },          
            { path: 'new-project', component: NewProjectComponent, canActivate: [NewProjectGuard], resolve: { attendants: GetAttendantsForProjectResolver } },
            { path: 'users', component: ManageUsersComponent, canActivate: [ManageUsersGuard] }       
        ]
    }    
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class DashboardRoutingModule {}
