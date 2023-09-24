import { Directive } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator } from '@angular/forms';

@Directive({
  selector: '[appDateValidator]',
  providers: [{provide: NG_VALIDATORS, useExisting: DateValidatorDirective, multi: true}]
})
export class DateValidatorDirective implements Validator {

  constructor() { }

  validate(control: AbstractControl): ValidationErrors | null {
    if (!control.value ) {
      return null;
    }
    const controlDate = new Date(control.value.year, control.value.month - 1, control.value.day, 12);
    const now = new Date();
    now.setDate(now.getDate() - 1);
    return controlDate <= now ? {invalidDate: control.value} : null;
  }

}
