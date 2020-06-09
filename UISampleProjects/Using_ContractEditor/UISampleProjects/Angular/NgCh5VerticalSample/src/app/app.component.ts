import { TranslationService } from './shared/translation.service';
import { SharedService } from './shared/shared.service';
import { ConfigService } from './shared/config.service';
import {
  Component,
  OnInit,
  AfterViewInit,
  HostBinding,
  ElementRef,
  OnDestroy,
  HostListener
} from '@angular/core';
declare var CrComLib: any;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, AfterViewInit, OnDestroy {
  @HostBinding('class') componentCssClass = 'light-theme';
  private swiperObj: any = [];
  public mainSlider: any;
  public activeIndex = 0;
  private triggerView: any;
  public windowSize: string;
  isSidebarOpen: boolean;
  crComLibTranslator = CrComLib.translationFactory.translator;
  languageTimer: any;
  public langData: object = [];
  private currentLng: string = 'en';
  private previousLanguage: string = '';
  private previousActiveIndex: number;
  private navItemSize: number;

  constructor(
    private configService: ConfigService,
    private sharedService: SharedService,
    private elem: ElementRef,
    private translationService: TranslationService
  ) { }

  ngOnInit() {

    // fetching current language name
    this.sharedService.newData.subscribe(val => {
      if (val) {
        // update selected language
        if (this.previousLanguage !== val) {
          this.getLanguage(val);
          this.crComLibTranslator.changeLanguage(val);
        }
        this.previousLanguage = val;

        // fetching swiper slides and translate
        this.swiperObj = this.langData[val].translation.swiperSlides;

        // published bottom navigation size
        if (this.swiperObj.length) {
          this.swiperObj.map((swiper: any, idx: number) => {
            CrComLib.publishEvent('s', 'nav_label_' + (idx), swiper.thumbTitle);
          });
        }

        // add a small delay for the buttons to get created before we add listeners first time
        setTimeout(() => {
          if (this.navItemSize !== this.swiperObj.length) {
            this.navItemSize = this.swiperObj.length;
            CrComLib.publishEvent('n', 'nav.items.size', this.navItemSize);
            for (let index = 0; index < this.navItemSize; index++) {
              this.addNavItemClickListener(index);
            }
          }
        }, 100);
      }
    });

    // main slider
    this.triggerView = this.elem.nativeElement.querySelector('.main-swiper');
    // oninit
    this.onResize();
    // initing ch5 translate
    this.initCh5LibTranslate();
  }

  // init ch5 translation
  private initCh5LibTranslate() {
    this.crComLibTranslator.init({
      fallbackLng: 'en',
      language: this.currentLng,
      debug: true,
      resources: this.langData
    });
  }

  // geting language data
  private getLanguage(lng: string) {
    this.translationService.getTranslation(lng).subscribe(val => {
      this.langData[lng] = { translation: val };
    });
  }

  ngAfterViewInit() {
    // get active slide index
    this.triggerView.addEventListener('select', event => {
      this.activeIndex = event.detail;
      this.navActiveState(this.activeIndex);
    });

    // On first load make the first button active
    this.navActiveState(this.activeIndex);
  }

  // navigate the page
  private addNavItemClickListener(idx: number) {
    const itemElem: HTMLElement = document.querySelector('.thumb-btn-' + String(idx));
    if (!!itemElem) {
      itemElem.addEventListener('click', () => {
        if (this.triggerView !== null && idx !== this.activeIndex) {
          setTimeout(() => {
            this.triggerView.setActiveView(idx);
            return;
          });
        }
      });
    }
  }

  // setting active state for bottom navigation
  private navActiveState(idx: number) {
    CrComLib.publishEvent('b', 'active_state_class_' + this.previousActiveIndex, false);
    if (this.activeIndex === idx) {
      this.previousActiveIndex = idx;
      CrComLib.publishEvent('b', 'active_state_class_' + idx, true);
    }
  }

  // getting header event
  public getHeaderEvent(event: any) {
    this.onSetTheme(event.cssClass);
    this.isSidebarOpen = event.isSidebarOpen;
  }

  // collapse sidebar on document click
  @HostListener('document:click', ['$event'])
  public hideSidebar(event: any) {
    if (event.target.id === 'sidebarToggle') {
      event.stopPropagation();
    } else {
      this.isSidebarOpen = false;
    }
  }

  // set theme class
  public onSetTheme(theme: any) {
    clearTimeout(this.languageTimer);
    this.languageTimer = setTimeout(() => {
      this.componentCssClass = theme;
      if (theme === 'dark-theme') {
        CrComLib.publishEvent('s', 'common_background_url', './assets/img/ch5-stone-dark-bg.jpg');
      } else {
        CrComLib.publishEvent('s', 'common_background_url', './assets/img/ch5-stone-light-bg.jpg');
      }
    }, 500);
  }

  // get window width
  @HostListener('window:resize', [''])
  public onResize() {
    if (window.innerWidth < 768) {
      this.windowSize = 'mobile';
    } else {
      this.windowSize = 'desktop';
    }
  }

  ngOnDestroy() { }
}
