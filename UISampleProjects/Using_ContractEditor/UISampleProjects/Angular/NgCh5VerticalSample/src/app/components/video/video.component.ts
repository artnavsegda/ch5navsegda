import { Component, OnInit, HostListener, Input } from '@angular/core';
declare var CrComLib: any;

@Component({
  selector: 'app-video',
  templateUrl: './video.component.html',
  styleUrls: ['./video.component.scss']
})
export class VideoComponent implements OnInit {
  public isCollapsed: boolean;
  @Input() langData: any;
  private videoScrollElement: any;
  private selectedSubscriptions = new Array;
  private previousItemsCount: number = 0;
  readonly CAMERA_SELECT = 'CameraList.SetSelectedCameraIndex';
  readonly CAMERA_INDEX = 'CameraList.IndexOfSelectedCamera';
  readonly CAMERA_LIST = 'CameraList.NumberOfCameras';

  constructor() {
  }

  ngOnInit() {
  }

  ngAfterViewInit() {
    this.videoScrollElement = document.getElementById('videoScrollerPanel');
    setTimeout(() => {
      this.videoInit();
    }, 100);
  }

  /**
   * Adding event listeners for all the camera list items
   * @param {number} numberOfCameras Camera list
   */
  addClickListeners() {
    this.videoScrollElement.addEventListener('click', function (event: any) {
      let clickedItem: any;
      clickedItem = Number(event.target.id.replace('video-list-item-', ''));
      if (!isNaN(clickedItem)) {
        CrComLib.publishEvent('n', this.CAMERA_SELECT, clickedItem);
      }
    }.bind(this));
  }

  /**
   * Remove the subscriptions when contact list size changes and decreases
   * @param {number} previousItemsCount previous number of items
   * @param {number} noItems current items
   */
  removeSubscriptions(previousItemsCount: number, noItems: number) {
    for (let idx = previousItemsCount - 1; idx >= noItems; idx--) {
      // unsubscribe to selected state
      CrComLib.unsubscribeState('n', this.CAMERA_INDEX, this.selectedSubscriptions[idx]);
      this.selectedSubscriptions.splice(idx, 1);
    }
  }

  /**
   * Add the subsriptions when contact list changes and size increases
   * @param {number} previousItemsCount  previous number of items
   * @param {number} noOfCameras current items
   */
  private addSubscriptions(previousItemsCount: number, noOfCameras: number) {
    for (let idx = previousItemsCount; idx < noOfCameras; idx++) {
      // subscribe to selected state
      this.selectedSubscriptions[idx] = this.selectedSubscriptions[idx] = CrComLib.subscribeState('n', this.CAMERA_INDEX, (selectedIndex) => {
        let listItem = document.getElementById('video-list-item-' + selectedIndex);
        if (!!listItem) {
          const activeNode = this.videoScrollElement.getElementsByClassName('active')[0];
          if (!!activeNode) activeNode.classList.remove('active'); // remove active list item
          listItem.classList.add('active');
        }
      });
    }
  }

  /**
   * Initialize once during the page or emulator load
   */
  private videoInit() {
    // Subscribe camera count
    CrComLib.subscribeState('n', this.CAMERA_LIST, (noOfCameras: number) => {
      this.previousItemsCount < noOfCameras ? this.addSubscriptions(this.previousItemsCount, noOfCameras) : this.removeSubscriptions(this.previousItemsCount, noOfCameras);
      this.previousItemsCount = noOfCameras;
    });

    // Add click listener for the video list
    this.addClickListeners();
  }

  // on document click
  @HostListener('document:click', ['$event'])
  documentClick(event: any): void {
    if (event.target.id === 'videoToggle') {
      event.stopPropagation();
    } else {
      this.isCollapsed = false;
    }
  }
}
