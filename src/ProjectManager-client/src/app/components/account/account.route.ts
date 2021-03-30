import { Routes, RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';

import { AccountComponent } from './account.app.component';
import { LogInComponent } from './log-in/log-in.component';
import { RegisterComponent } from './register/register.component';
import { HomeGuard } from './guards/home.guard';

const routes: Routes = [
    {
        path: '', component: AccountComponent, canActivate: [HomeGuard],
        children: [
            { path: 'login', component: LogInComponent },
            { path: 'register', component: RegisterComponent }
        ]
    }
    //{ path: 'path/:routeParam', component: MyComponent },
    //{ path: 'staticPath', component: ... },
    //{ path: '**', component: ... },
    //{ path: 'oldPath', redirectTo: '/staticPath' },
    //{ path: ..., component: ..., data: { message: 'Custom' }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class AccountRoutingModule {}
