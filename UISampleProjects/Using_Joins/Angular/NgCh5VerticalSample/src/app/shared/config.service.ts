import { Ch5Emulator } from '@crestron/ch5-crcomlib/build_bundles/umd/@types';
import { Injectable } from '@angular/core';
declare var CrComLib: any;

@Injectable()
export class ConfigService {
  private ch5Emulator: Ch5Emulator = CrComLib.Ch5Emulator.getInstance();
  constructor() { }

  // init emulator
  public initEmulator(emulator: any) {
    CrComLib.Ch5Emulator.clear();
    this.ch5Emulator = CrComLib.Ch5Emulator.getInstance();
    this.ch5Emulator.loadScenario(emulator);
    this.ch5Emulator.run();
  }
}
