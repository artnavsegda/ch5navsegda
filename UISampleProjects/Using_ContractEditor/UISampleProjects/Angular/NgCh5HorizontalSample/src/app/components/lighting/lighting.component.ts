import {
  Component,
  AfterViewInit,
  ElementRef,
  OnInit,
  Input,
  HostListener
} from '@angular/core';
declare var CrComLib: any;

@Component({
  selector: 'app-lighting',
  templateUrl: './lighting.component.html',
  styleUrls: ['./lighting.component.scss']
})
export class LightingComponent implements OnInit, AfterViewInit {
  public isCollapsed: boolean;
  @Input() langData: any;

  constructor(private elem: ElementRef) { }

  ngOnInit() {

  }

  ngAfterViewInit() {
    setTimeout(() => {
      this.addButtonClasses();
    }, 3000);
  }

  private addButtonClasses() {
    const elements = this.elem.nativeElement.querySelectorAll('.lighting-button:not([disabled])');
    if (elements) {
      elements.forEach((element: any) => {
        element.addEventListener('press', (e: any) => {
          e.currentTarget.classList.add('pulse-once-lighting');
          const myButton = e.currentTarget;
          setTimeout(function () {
            myButton.classList.remove('pulse-once-lighting');
          }, 1500);
        });
      });
    }
  }

  // on document click
  @HostListener('document:click', ['$event'])
  documentClick(): void {
    this.isCollapsed = false;
  }
}
