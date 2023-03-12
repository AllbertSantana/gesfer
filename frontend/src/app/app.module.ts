import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { AppRoutingModule } from './app-routing.module';
import { FuncionarioModule } from './funcionarios/funcionarios.module';
import { HttpClientModule } from '@angular/common/http';
import { HomeComponent } from './home/home.component';
import { MaterialModule } from 'src/material.module';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { PtBrMatPaginatorIntl } from './shared/implementations/mat-paginator-intl/pt-br-mat-paginator-intl';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MessageSnackBarComponent } from './shared/components/message-snack-bar/message-snack-bar/message-snack-bar.component';
import { ReactiveFormsModule } from '@angular/forms';
import { LoginComponent } from './login/login.component';
import { httpInterceptorProviders } from './shared/interceptors';
import { TextMaskModule } from 'angular2-text-mask';

@NgModule({
  declarations: [
    AppComponent,
    PageNotFoundComponent,
    HomeComponent,
    MessageSnackBarComponent,
    LoginComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    MaterialModule,
    FuncionarioModule,
    AppRoutingModule,
    ReactiveFormsModule,
    TextMaskModule
  ],
  providers: [
    httpInterceptorProviders,
    { provide: MatPaginatorIntl, useClass: PtBrMatPaginatorIntl }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }