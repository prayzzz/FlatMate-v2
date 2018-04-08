import Dictionary from "../../shared/Dictionary";

export interface IDragItem {
    dragStart(): void;

    dragEnd(): void;

    dragEnter(): void;

    dragLeave(): void;
}

export class ListActionManager<T extends IDragItem> {
    private readonly onSwipeEnd: (item: any) => void;
    private readonly onSortCompleted: (i: number, e: T) => void;

    private isSwiping: boolean;
    private swipeStartX: number;
    private swipeElement: HTMLElement | null;

    private enterLeaveCount: Dictionary<Element, number>;
    private draggedItem: T | null;
    private draggedItemElement: Element | null;
    private dragZoneElement: Element | null;
    private dragPlaceHolder: HTMLElement;

    constructor(
        onSwipeEnd: (item: any) => void,
        onSortCompleted: (i: number, item: T) => void
    ) {
        this.onSwipeEnd = onSwipeEnd;
        this.onSortCompleted = onSortCompleted;

        this.isSwiping = false;
        this.swipeStartX = 0;
        this.swipeElement = null;

        this.enterLeaveCount = new Dictionary();
        this.draggedItem = null;
        this.draggedItemElement = null;
        this.dragZoneElement = null;

        this.dragPlaceHolder = this.getPlaceholder();
    }

    private getPlaceholder(): HTMLElement {
        const temp = document.getElementById("drag-placeholder-template");
        if (temp === null) {
            throw "'drag-placeholder-template' not found";
        }

        return new DOMParser().parseFromString(
            temp.innerText.trim(),
            "text/html"
        ).body.firstChild as HTMLElement;
    }

    public zoneHandlers(): any {
        return {
            drop: (vm: any, event: any) => this.drop(vm, event),
            dragover: (vm: any, event: any) => {
                this.allowDrop(event);
            }
        };
    }

    public itemHandlers(): any {
        return {
            dragover: (vm: any, event: any) => this.dragOver(vm, event),
            dragenter: (vm: any, event: any) => this.dragEnter(vm, event),
            dragleave: (vm: any, event: any) => this.dragLeave(vm, event),
            touchstart: (vm: any, event: any) => this.touchStart(vm, event),
            touchmove: (vm: any, event: any) => this.touchMove(vm, event),
            touchend: (vm: any, event: any) => this.touchEnd(vm, event)
        };
    }

    public handleHandlers(): any {
        return {
            dragstart: (vm: any, event: any) => this.dragStart(vm, event),
            dragend: (vm: any, event: any) => this.dragEnd(vm, event)
        };
    }

    private touchStart(item: any, event: TouchEvent): boolean {
        this.swipeElement = event.currentTarget as HTMLElement;
        this.swipeStartX = event.changedTouches[0].pageX;

        this.swipeElement!!.style.transition = "transform 0ms";
        this.swipeElement!!.style.transform = "translate3d(0px, 0px, 0px)";
        this.isSwiping = true;

        return true;
    }

    private touchEnd(item: any, event: TouchEvent): boolean {
        if (!this.isSwiping) {
            return true;
        }

        const delta = event.changedTouches[0].pageX - this.swipeStartX;
        const elWidth = this.swipeElement!!.getBoundingClientRect().width;

        if (elWidth * 0.25 > delta) {
            // Threshold not reached, move back to original position
            this.swipeElement!!.style.transition = "transform 200ms";
            this.swipeElement!!.style.transform = "translate3d(0px, 0px, 0px)";

            const swipElement = this.swipeElement;
            setTimeout(() => {
                swipElement!!.style.transition = "";
                swipElement!!.style.transform = "";
            }, 200);
        } else {
            // Threshold reached
            const x = this.swipeElement!!.getBoundingClientRect().width;

            this.swipeElement!!.style.transition = "transform 200ms";
            this.swipeElement!!.style.transform = `translate3d(${x}px, 0px, 0px)`;

            setTimeout(() => this.onSwipeEnd(item), 200);
        }

        this.swipeStartX = 0;
        this.isSwiping = false;
        this.swipeElement = null;

        return true;
    }

    private touchMove(item: any, event: TouchEvent): boolean {
        if (!this.isSwiping) {
            return true;
        }

        const delta = event.changedTouches[0].pageX - this.swipeStartX;
        this.swipeElement!!.style.transition = "transform 0ms";
        this.swipeElement!!.style.transform = `translate3d(${delta}px, 0px, 0px)`;

        return true;
    }

    private dragStart(dragItem: T, event: DragEvent): boolean {
        const self = this;

        const itemElement = this.findParentElementWithClass(
            "drag-item",
            event.srcElement
        ) as HTMLElement | null;

        if (itemElement == null) {
            return false;
        }

        // const x = event.pageX - itemElement.offsetLeft;
        // const y = event.pageY - itemElement.offsetTop;

        event.dataTransfer.effectAllowed = "move";
        event.dataTransfer.setDragImage(itemElement, 0, 0);

        if (dragItem.dragStart) {
            dragItem.dragStart();
        }

        this.draggedItem = dragItem;
        this.draggedItemElement = itemElement;
        this.dragZoneElement = itemElement.parentElement;
        this.dragPlaceHolder.style.height = itemElement.clientHeight + "px";
        this.enterLeaveCount.clear();

        setTimeout(function () {
            self.draggedItemElement!!.classList.add("drag-hide");

            itemElement.parentNode!!.insertBefore(
                self.dragPlaceHolder,
                itemElement.nextSibling
            );
        }, 1);

        return true;
    }

    private dragEnd(dragItem: IDragItem, event: DragEvent): boolean {
        if (this.draggedItem == null) {
            return true;
        }

        if (dragItem.dragEnd) {
            dragItem.dragEnd();
        }

        this.removePlaceholder(this.dragZoneElement);
        this.draggedItemElement!!.classList.remove("drag-hide");

        this.draggedItem = null;
        this.draggedItemElement = null;
        this.dragZoneElement = null;

        return true;
    }

    private drop(dragItem: IDragItem, event: DragEvent): boolean {
        if (this.draggedItem == null) {
            return true;
        }

        const el = event.currentTarget!! as HTMLElement;
        if (el != null) {
            // find new index
            let newIndex = 0;
            for (let i = 0; i < el.children.length; i++) {
                const curEl = el.children.item(i);
                if (curEl == this.dragPlaceHolder) {
                    break;
                } else if (
                    curEl.classList.contains("drag-item") &&
                    curEl != this.draggedItemElement
                ) {
                    newIndex++;
                }
            }

            if (this.onSortCompleted && this.draggedItem) {
                this.onSortCompleted(newIndex, this.draggedItem);
            }
        }

        return true;
    }

    private dragEnter(dragItem: IDragItem, event: DragEvent): boolean {
        if (this.draggedItem == null) {
            return true;
        }

        const enterCount = this.enterElement(event.srcElement);
        if (enterCount == 1 && dragItem.dragEnter) {
            dragItem.dragEnter();
        }

        return true;
    }

    private dragLeave(dragItem: IDragItem, event: DragEvent): boolean {
        if (this.draggedItem == null) {
            return true;
        }

        const leaveCount = this.leaveElement(event.srcElement);
        if (leaveCount == 0 && dragItem.dragLeave) {
            dragItem.dragLeave();
        }

        return true;
    }

    private dragOver(dragItem: IDragItem, event: DragEvent): boolean {
        if (this.draggedItem == null) {
            return true;
        }

        this.allowDrop(event);

        const el = this.findParentElementWithClass(
            "drag-item",
            event.srcElement
        ) as HTMLElement | null;

        if (el == null) {
            return true;
        }

        // !! calculate before remove placeholder
        const top =
            el.getBoundingClientRect().top + el.offsetHeight / 2 - event.pageY;

        this.removePlaceholder(el);

        // add placehodler before or after
        if (top > 0) {
            el.parentNode!!.insertBefore(this.dragPlaceHolder, el);
        } else {
            el.parentNode!!.insertBefore(
                this.dragPlaceHolder,
                el.nextElementSibling
            );
        }

        return true;
    }

    private removePlaceholder(el: Element | null): void {
        if (el == null) {
            return;
        }

        try {
            el.removeChild(this.dragPlaceHolder);
        } catch (e) {
        }
    }

    private allowDrop(event: DragEvent) {
        event.preventDefault();
    }

    private enterElement(el: Element | null): number {
        if (el == null) {
            return 0;
        }

        const counter = this.enterLeaveCount.get(el);
        if (counter == null) {
            this.enterLeaveCount.set(el, 0);
        }

        const count = this.enterLeaveCount.get(el)!! + 1;
        this.enterLeaveCount.set(el, count);

        return count;
    }

    private leaveElement(el: Element | null): number {
        if (el == null) {
            return 0;
        }

        const counter = this.enterLeaveCount.get(el);
        if (counter == null) {
            this.enterLeaveCount.set(el, 0);
        }

        const count = this.enterLeaveCount.get(el)!! - 1;
        this.enterLeaveCount.set(el, count);

        return count;
    }

    private findParentElementWithClass(
        cssClass: string,
        srcElement: Element | null
    ): Element | null {
        let el = srcElement;
        while (el && !el.classList.contains(cssClass)) {
            el = el.parentElement;
        }

        return el;
    }
}
