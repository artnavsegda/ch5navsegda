import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
@Injectable()
export class SharedService {
  public dataSource = new BehaviorSubject('');
  newData = this.dataSource.asObservable();

  constructor() { }

  // send data from component to other component
  public changeData(data: any) {
    this.dataSource.next(data);
  }
}
