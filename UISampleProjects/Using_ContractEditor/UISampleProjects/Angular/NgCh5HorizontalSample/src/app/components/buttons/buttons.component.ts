import { Component, ElementRef, OnInit } from '@angular/core';

@Component({
    selector: 'app-buttons',
    templateUrl: './buttons.component.html',
    styleUrls: ['./buttons.component.scss']
})
export class ButtonsComponent implements OnInit {
    constructor(private elem: ElementRef) { }

    ngOnInit() {
        this.addButtonClasses('.shadow-pulse-button:not([disabled])', 'shadow-pulse-button-once');
        this.addButtonClasses('.shadow-pulse-gradient-button:not([disabled])', 'shadow-pulse-gradient-button-once');
        this.addButtonClasses('.outline-animate-button:not([disabled])', 'outline-animate-button-once');
    }

    addButtonClasses(gatherElementsClass: any, appendClass: any) {
        const elements = this.elem.nativeElement.querySelectorAll(
            gatherElementsClass
        );
        if (elements) {
            elements.forEach((element: any) => {
                element.addEventListener('click', (e: any) => {
                    e.currentTarget.classList.add(appendClass);
                    const myButton = e.currentTarget;
                    setTimeout(() => {
                        myButton.classList.remove(appendClass);
                    }, 1001);
                });
            });
        }
    }
}
