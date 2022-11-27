import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { AppRoutingModule } from './app-routing.module';
import { FuncionariosModule } from './funcionarios/funcionarios.module';
import { HttpClientModule } from '@angular/common/http';
import { HomeComponent } from './home/home.component';
import { MaterialModule } from 'src/material.module';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { PtBrMatPaginatorIntl } from './shared/implementations/mat-paginator-intl/pt-br-mat-paginator-intl';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  declarations: [
    AppComponent,
    PageNotFoundComponent,
    HomeComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    MaterialModule,
    FuncionariosModule,
    AppRoutingModule
  ],
  providers: [
    { provide: MatPaginatorIntl, useClass: PtBrMatPaginatorIntl }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
