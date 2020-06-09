import { Injectable } from '@angular/core';

@Injectable()
export class ScrollService {
  constructor() { }

  scrollDetect(element: any) {
    if (
      element.nativeElement.scrollHeight >
      element.nativeElement.clientHeight + element.nativeElement.scrollTop
    ) {
      return true;
    } else {
      return false;
    }
  }
}
