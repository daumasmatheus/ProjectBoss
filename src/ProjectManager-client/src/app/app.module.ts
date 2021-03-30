import { BrowserModule } from '@angular/platform-browser';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FlexLayoutModule } from '@angular/flex-layout';
import { SocialAuthServiceConfig, SocialLoginModule } from 'angularx-social-login';
import { GoogleLoginProvider } from 'angularx-social-login';
import { NgxUiLoaderConfig, NgxUiLoaderHttpModule, NgxUiLoaderModule, NgxUiLoaderRouterModule } from 'ngx-ui-loader';

import { MAT_DATE_LOCALE } from '@angular/material/core';

import { AngularMaterialModule } from './angular-material.module';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SnackBarHelper } from './helpers/snack-bar.helper';
import { NavigationModule } from './components/navigation/navigation.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { PopConfirmationComponent } from './helpers/pop-confirmation/pop-confirmation.component';
import { DownloadFileDialogComponent } from './helpers/download-file-dialog/download-file-dialog.component';
import { HttpClientModule } from '@angular/common/http';
import { ChartsModule } from 'ng2-charts';

const ngxUiLoaderConfig: NgxUiLoaderConfig = {
  bgsColor: '#fff',
  fgsColor: '#fff',
  overlayColor: "rgba(40, 40, 40, 0.8)",
  fgsType: "cube-grid",
  pbColor: "#fff",
  blur: 5
}

@NgModule({
  declarations: [
    AppComponent,
    PopConfirmationComponent,
    DownloadFileDialogComponent               
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    AngularMaterialModule,
    FlexLayoutModule,
    SocialLoginModule,
    HttpClientModule,
    NgxUiLoaderModule.forRoot(ngxUiLoaderConfig),
    NgxUiLoaderHttpModule,
    NgxUiLoaderRouterModule,
    NavigationModule,
    FontAwesomeModule
  ],
  providers: [
    SnackBarHelper,
    { provide: MAT_DATE_LOCALE, useValue: 'pt-BR' },    
    {
      provide: 'SocialAuthServiceConfig',
      useValue: {
        autoLogin: false,
        providers: [
          {
            id: GoogleLoginProvider.PROVIDER_ID,
            provider: new GoogleLoginProvider('1084083418579-2bhhi5vq01qvn65vmgu6gt123vmfl9jv.apps.googleusercontent.com')
          }
        ]
      } as SocialAuthServiceConfig      
    }
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }
