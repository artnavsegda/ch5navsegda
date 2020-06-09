declare module '*.json' {
  const value: any;
  export default value;
}
declare var System: System;
interface System {
  import(request: string): Promise<any>;
}
