import { Component, OnInit, ViewChild, Input, HostListener } from '@angular/core';

declare var CrComLib: any;

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.scss']
})
export class ListsComponent implements OnInit {
  public isCollapsed: boolean;
  @ViewChild('GetPosition') getPosition: any;
  public scrollMaxHeight: string;
  @Input() windowSize: string;
  public activeIndex = 0;
  private listScrollElement: any;
  private selectedSubscriptions = new Array;
  private previousItemsCount = 0;

  constructor() {
  }

  ngOnInit() { }

  ngAfterViewInit() {
    this.listScrollElement = document.getElementById('listScrollerPanel') as HTMLElement;
    setTimeout(() => {
      this.listsInit();
    }, 100);
  }

  /**
   * This is private method, it invokes on list click.
   */
  private addContactItemClickListener() {
    this.listScrollElement.addEventListener('click', function (event: any) {
      let clickedItem: any;
      clickedItem = Number(event.target.id.replace('contact-list-item-', ''));
      if (!isNaN(clickedItem)) {
        this.clickedContactList(clickedItem);
      }
    }.bind(this));
  }

  /**
   * This method setting active class on list
   * @param {number} idx is index of list item
   */
  public clickedContactList(idx: number) {
    let listItem = document.getElementById('contact-list-item-' + String(idx));
    if (!!listItem && !listItem.classList.contains('active')) {
      const eventName = `ContactList.Contact[${idx}].SetContactSelected`;
      CrComLib.publishEvent('b', eventName, true);
      CrComLib.publishEvent('b', eventName, false);
    }
  }

  /**
   * Remove the subscriptions when contact list size changes and decreases
   * @param {number} previousItemsCount previous number of items
   * @param {number} noItems current items
   */
  private removeSubscriptions(previousItemsCount: number, noItems: number) {
    for (let idx = previousItemsCount - 1; idx >= noItems; idx--) {
      // unsubscribe to selected state
      CrComLib.unsubscribeState('b', `ContactList.Contact[${idx}].ContactIsSelected`, this.selectedSubscriptions[idx]);
      this.selectedSubscriptions.splice(idx, 1);
    }
  }

  /**
   * Add the subsriptions when contact list changes and size increases
   * @param {number} previousItemsCount  previous number of items
   * @param {number} noItems current items
   */
  private addSubscriptions(previousItemsCount: number, noItems: number) {
    for (let idx = previousItemsCount; idx < noItems; idx++) {
      // subscribe to selected state
      this.selectedSubscriptions[idx] = this.selectedSubscriptions[idx] = CrComLib.subscribeState('b', `ContactList.Contact[${idx}].ContactIsSelected`, (isSelected) => {
        let listItem = document.getElementById('contact-list-item-' + idx);
        if (!!listItem) {
          if (isSelected) {
            const activeNode = this.listScrollElement.getElementsByClassName('active')[0];
            if (!!activeNode) activeNode.classList.remove('active'); // remove active list item
            listItem.classList.add('active');
            this.activeIndex = idx;
          }
        }
      });
    }
  }

  /**
   * Setting height of the list and loading the contact list
   */
  private listsInit() {
    // subscribe to the number of items in the list. Keep the subscription on.
    CrComLib.subscribeState('n', 'ContactList.NumberOfContacts', (noItems: number) => {
      this.previousItemsCount < noItems ? this.addSubscriptions(this.previousItemsCount, noItems) : this.removeSubscriptions(this.previousItemsCount, noItems);
      this.previousItemsCount = noItems;
    });

    this.addContactItemClickListener();
    // setting list height
    this.setListHeight();
  }

  // contact details show-more in smaller device
  public toggleDetail() {
    this.isCollapsed = !this.isCollapsed;
    setTimeout(() => {
      this.setListHeight();
    }, 301);
  }

  // get max height of list resize and orientationchange
  @HostListener('window:resize', [''])
  @HostListener('window:orientationchange', [''])
  private setListHeight() {
    setTimeout(() => {
      const excludeSpace = this.windowSize === 'desktop' ? 164 : 105;
      this.scrollMaxHeight = window.innerHeight - (this.getPosition.nativeElement.getBoundingClientRect().top + excludeSpace) + 'px';
    }, 30);
  }
}
