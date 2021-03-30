import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LogInComponent } from './log-in/log-in.component';
import { RegisterComponent } from './register/register.component';
import { AccountRoutingModule } from './account.route';
import { AccountComponent } from './account.app.component';
import { AccountService } from './services/account.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AngularMaterialModule } from 'src/app/angular-material.module';
import { FlexLayoutModule } from '@angular/flex-layout';
import { HomeGuard } from './guards/home.guard';

@NgModule({
    declarations: [
        AccountComponent,
        LogInComponent,
        RegisterComponent
    ],
    imports: [ 
        CommonModule,
        AccountRoutingModule,
        FormsModule,
        ReactiveFormsModule,
        HttpClientModule,
        AngularMaterialModule,
        FlexLayoutModule
    ],
    exports: [],
    providers: [
        HomeGuard,
        AccountService        
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AccountModule {}