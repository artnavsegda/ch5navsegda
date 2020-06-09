import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import * as enContent from 'src/assets/data/translation/en.json';
import * as frContent from 'src/assets/data/translation/fr.json';
import * as deContent from 'src/assets/data/translation/de.json';
import * as nlContent from 'src/assets/data/translation/nl.json';
import * as esContent from 'src/assets/data/translation/es.json';
const TRANSLATIONS = {
  en: enContent,
  fr: frContent,
  de: deContent,
  nl: nlContent,
  es: esContent
};

@Injectable()
export class TranslationService {
  constructor() {}

  getTranslation(lang: string): Observable<any> {
    return of(TRANSLATIONS[lang].default);
  }
}
