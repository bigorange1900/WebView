import * as React from "react";
import "css!./ExampleView.css";

export interface ISomeType {
    name: string;
}

export interface IExampleViewProperties {
    click(arg: ISomeType): void;
    canDrop(data: string): Promise<string>;
}

export interface IExampleViewBehaviors {
    callMe(strValue: string, boolValue: boolean, floatValue: number): void;
}

export default class ExampleView extends React.Component<IExampleViewProperties, {}> implements IExampleViewBehaviors {

    private dropEffect: string;

    callMe(strValue: string, boolValue: boolean, floatValue: number): void {
        alert("React View says: clicked on a WPF button");
    }

    private onDrag(e: React.DragEvent<HTMLButtonElement>) {
        e.dataTransfer.effectAllowed = "all";
        e.dataTransfer.setData("text/plain", "drag data");
    }

    private async onDragEnter(e: React.DragEvent<HTMLDivElement>, effectAllowed: string) {
        this.dropEffect = await this.props.canDrop(effectAllowed);
    }

    private async onDragOver(e: React.DragEvent<HTMLDivElement>) {
        e.dataTransfer.dropEffect = this.dropEffect;
        e.preventDefault();
    }

    private onDrop(e: React.DragEvent<HTMLDivElement>) {
        alert("Dropped: " + e.dataTransfer.getData("text/plain"));
    }

    private renderDropZone(effectAllowed: string) {
        return <div
            className="dropzone"
            onDragEnter={(e) => this.onDragEnter(e, effectAllowed)}
            onDragOver={(e) => this.onDragOver(e)}
            onDragLeave={(e) => this.dropEffect = null}
            onDrop={(e) => this.onDrop(e)}>{effectAllowed} drop zone</div>;
    }

    render() {
        return (
            <>
                <button draggable onDrag={(e) => this.onDrag(e)} onClick={() => this.props.click(null)}>Click me!</button>
                <br/>
                <br/>
                {this.renderDropZone("copy")}
                {this.renderDropZone("move")}
                {this.renderDropZone("link")}
                {this.renderDropZone("none")}
            </>
        );
    }
}