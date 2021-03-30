import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { fab, faGitSquare, faLinkedin } from '@fortawesome/free-brands-svg-icons';
import { FaIconLibrary, FontAwesomeModule } from '@fortawesome/angular-fontawesome';

import { AccessDeniedComponent } from './access-denied/access-denied.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { FooterComponent } from './footer/footer.component';
import { AngularMaterialModule } from 'src/app/angular-material.module';

@NgModule({
    declarations: [
        AccessDeniedComponent, 
        NotFoundComponent,
        FooterComponent
    ],
    imports: [ 
        CommonModule,
        RouterModule,
        AngularMaterialModule, 
        FontAwesomeModule 
    ],
    exports: [
        FooterComponent
    ],
    providers: [],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class NavigationModule {
    constructor(private library: FaIconLibrary){
        library.addIcons(faLinkedin, faGitSquare)
    }
}