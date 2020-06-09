/*jslint es6 */
/*global document, window, CrComLib, includeHtmlTpl */
var listsModule = (function () {
    'use strict';
    /**
     * All public properties defined over here
     */
    let listScrollElement = document.getElementById('listScrollerPanel');
    let selectedSubscriptions = new Array;
    let previousItemsCount = 0;
    let activeIndex = 0;

    /**
     * This is private method, it invokes on list click.
     */
    function addContactItemClickListener() {
        listScrollElement.addEventListener('click', function (event) {
            let clickedItem;
            clickedItem = Number(event.target.id.replace('contact-list-item-', ''));
            if (!isNaN(clickedItem)) clickedContactList(clickedItem);
        });
    }

    /**
     * This method setting active class on list
     * @param {number} idx is index of list item
     */
    function clickedContactList(idx) {
        let listItem = document.getElementById('contact-list-item-' + idx);
        if (!!listItem && !listItem.classList.contains('active')) {
            let eventName = `ContactList.Contact[${idx}].SetContactSelected`;
            CrComLib.publishEvent('b', eventName, true);
            CrComLib.publishEvent('b', eventName, false);
        }
    }

    /**
     * Remove the subscriptions when contact list size changes and decreases
     * @param {number} previousItemsCount previous number of items
     * @param {number} noItems current items
     */
    function removeSubscriptions(previousItemsCount, noItems) {
        for (let idx = previousItemsCount - 1; idx >= noItems; idx--) {
            // unsubscribe to selected state
            CrComLib.unsubscribeState('b', `ContactList.Contact[${idx}].ContactIsSelected`, selectedSubscriptions[idx]);
            selectedSubscriptions.splice(idx, 1);
        }
    }

    /**
     * Add the subsriptions when contact list changes and size increases
     * @param {number} previousItemsCount  previous number of items
     * @param {number} noItems current items
     */
    function addSubscriptions(previousItemsCount, noItems) {
        for (let idx = previousItemsCount; idx < noItems; idx++) {
            // subscribe to selected state
            selectedSubscriptions[idx] = selectedSubscriptions[idx] = CrComLib.subscribeState('b', `ContactList.Contact[${idx}].ContactIsSelected`, function (isSelected) {
                let listItem = document.getElementById('contact-list-item-' + idx);
                if (!!listItem) {
                    if (isSelected) {
                        const activeNode = listScrollElement.getElementsByClassName('active')[0];
                        if (!!activeNode) activeNode.classList.remove('active'); // remove active list item
                        listItem.classList.add('active')
                        activeIndex = idx;
                    }
                }
            });
        }
    }

    /**
     * Setting height of the list and loading the contact list
     */
    function listsInit() {
        // subscribe to the number of items in the list. Keep the subscription on.
        CrComLib.subscribeState('n', 'ContactList.NumberOfContacts', function (noItems) {
            previousItemsCount < noItems ? addSubscriptions(previousItemsCount, noItems) : removeSubscriptions(previousItemsCount, noItems);
            previousItemsCount = noItems;
        });

        addContactItemClickListener();
        // setting list height
        setListHeight();
    }

    /**
     * This is for calculate dynamic height of list and setting on the element
     */
    function setListHeight() {
        let excludeSpace;
        window.innerWidth > 767 ? excludeSpace = 44 : excludeSpace = 22;
        let scrollHeight = window.innerHeight - (listScrollElement.getBoundingClientRect().top + excludeSpace);
        document.querySelector('.list-scroller').setAttribute('maxHeight', scrollHeight + 'px');
    }

    /**
     * This for collapse contact detail
     */
    function toggleContact() {
        let detailElm = document.getElementById('contactDetail');
        detailElm.classList.toggle('open');
        if (detailElm.classList.contains('open')) {
            listScrollerElment.firstElementChild.classList.remove('iphone6-list');
        } else {
            listScrollerElment.firstElementChild.classList.add('iphone6-list');
        }
        setTimeout(function () {
            setListHeight();
        }, 301);
    }


    /**
     * This method will invoke on window resize and orientationchange
     */
    window.addEventListener('resize', setListHeight);
    window.addEventListener('orientationchange', setListHeight);

    /**
     * All public or private methods which need to call on init
     */
    let listsPage = document.querySelector('.lists-page');
    listsPage.addEventListener('afterLoad', listsInit);

    /**
     * All public method and properties exporting here
     */
    return {
        toggleContact: toggleContact
    };
}());