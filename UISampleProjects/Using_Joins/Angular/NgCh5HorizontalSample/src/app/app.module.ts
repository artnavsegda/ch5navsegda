import { VideoComponent } from './components/video/video.component';
import { SourceComponent } from './components/source/source.component';
import { SafeHtmlPipe } from './shared/safe-html.pipe';

import { TranslationService } from './shared/translation.service';
import { ConfigService } from './shared/config.service';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { AppComponent } from './app.component';
import { HeaderComponent } from './shared/header/header.component';
import { APP_BASE_HREF } from '@angular/common';
import { ButtonsComponent } from './components/buttons/buttons.component';
import { ListsComponent } from './components/lists/lists.component';
import { LightingComponent } from './components/lighting/lighting.component';
import { SharedService } from './shared/shared.service';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    ButtonsComponent,
    ListsComponent,
    SourceComponent,
    LightingComponent,
    SafeHtmlPipe,
    VideoComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule
  ],
  providers: [
    { provide: APP_BASE_HREF, useValue: './' },
    ConfigService,
    SharedService,
    TranslationService
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  bootstrap: [AppComponent]
})
export class AppModule {}
