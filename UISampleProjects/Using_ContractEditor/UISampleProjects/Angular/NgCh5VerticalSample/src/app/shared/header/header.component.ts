import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  HostListener
} from '@angular/core';
import { SharedService } from '../shared.service';
import * as AppVersionInfo from '.../../../package.json';

declare var CrComLib: any;

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})

export class HeaderComponent implements OnInit {
  @Output() valueChange = new EventEmitter();
  themeBtnActive = 'light-theme';
  menuShow: boolean;
  selectedLang = 'en';
  headerEvent = { cssClass: this.themeBtnActive, isSidebarOpen: false };
  languages = ['en', 'fr', 'de', 'nl', 'es'];
  languageTimeout: any;
  themeTimeout: any;

  // cr-com-lib version information.
  public crComLibVersion: string = CrComLib.version;
  public crComBuildDate: string = CrComLib.buildDate;
  public hasVersionInfo: boolean = false;

  // sample app version
  public ngAppVersion: string = AppVersionInfo['version'];
  public ngAppName: string = AppVersionInfo['name'];
  public hasAppVersion: boolean = false;

  constructor(private sharedService: SharedService) { }

  ngOnInit() {
    this.sharedService.changeData(this.selectedLang);
    if (!!this.crComLibVersion && !!this.crComBuildDate) {
      this.hasVersionInfo = true;
    }
    if (!!this.ngAppVersion && !!this.ngAppName) {
      this.ngAppName = this.transformAppName(this.ngAppName);
      this.hasAppVersion = true;
    }
  }

  /**
   * convert dashcase word to camelcase with first letter capitalize
   * @param {string} appName
   */
  private transformAppName(appName: string) {
    return appName[0].toUpperCase() + appName.replace(/-([a-z])/g, (a, b) => {
      return b.toUpperCase();
    }).slice(1);
  }


  // change language
  changeLanguage(lang) {
    this.selectedLang = lang;
   
    // restrict user clicking multiple times in a short duration
    clearTimeout(this.languageTimeout);

     // sending event on language change
    this.languageTimeout = setTimeout(() => {
      this.sharedService.changeData(lang);
    }, 1000);
  }

  // send theme name
  setTheme(theme) {
    this.headerEvent.cssClass = theme;
    this.valueChange.emit(this.headerEvent);
    this.themeBtnActive = theme;

    // restrict user clicking multiple times in a short duration
    this.themeTimeout = setTimeout(() => {
      this.valueChange.emit({ cssClass: theme });
    }, 1000);
  }

  // hide language menu when you click outside
  @HostListener('document:click', ['$event'])
  documentClick(event: any) {
    if (event.target.id === 'sidebarToggle') {
      event.stopPropagation();
    } else {
      this.headerEvent.isSidebarOpen = false;
    }
    this.menuShow = false;
  }

  // send sidebar status
  toggleSidebar() {
    this.headerEvent.isSidebarOpen = !this.headerEvent.isSidebarOpen;
    this.valueChange.emit(this.headerEvent);
  }
}
