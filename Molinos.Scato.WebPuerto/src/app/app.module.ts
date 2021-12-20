import { BrowserModule } from '@angular/platform-browser';
import { LOCALE_ID, NgModule } from '@angular/core';
import { routeConfig } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { CommonModule, DatePipe, registerLocaleData } from '@angular/common';
import localeEsAr from '@angular/common/locales/es-AR';
import { SidebarModule } from 'ng-sidebar';
import { SharedModule } from './shared/shared.module';
import { RouterModule } from '@angular/router';
import { SharedComponentModule } from './shared/componentes/shared-components.module';
import { MessageService } from 'primeng/api';

registerLocaleData(localeEsAr, 'es-Ar');

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    CommonModule,
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule,
    SharedModule,
    SharedComponentModule,
    CalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory,
    }),
    SidebarModule.forRoot(),
    RouterModule.forRoot(routeConfig)
  ],
  providers: [
    { provide: LOCALE_ID, useValue: 'es-Ar' },
    DatePipe,
    MessageService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
