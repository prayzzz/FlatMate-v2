namespace Flatmate.Areas.Lists {
    interface IITem {
        id: number;
        lastEditor: IUser;
        name: string;
        owner: IUser;
    }

    interface IUser {
        id: number;
        name: string;
    }

    interface IItemList {
        description: string;
        id: number;
        isPublic: boolean;
        itemCount: number;
        lastEditor: IUser;
        name: string;
        owner: IUser;
        items: Array<IITem>;
    }

    export class ItemListEditor {
        private model: IItemList;

        constructor() {
            const dataElement = document.getElementById("viewData");
            if (!dataElement) {
                throw "no data available";
            }

            this.model = JSON.parse(dataElement.innerText);

            console.log(this.model);
        }
    }
}