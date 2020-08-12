import React from "react";

import "./button.scss";

function Button(props) {

    const {
        isHollow,
        hasArrow,
        buttonType,
        text,
        url
    } = props;
    const containerClasses = (isHollow ? ["button_hollow"] : []);

    return (
        <div className={["button"].concat(containerClasses).join(" ")}>
            {
                buttonType === "a" ?
                    <a className="button__basis"
                        href={url ? url : "https://errorpage.com"} target={url ? "_blank" : ""} rel={url ? "noopener noreferrer" : ""}>
                        <span className="button__text">{text}</span>
                        {hasArrow ? <span className="button__arrow">arrow_forward</span> : null}

                    </a>

                    : buttonType === "button" ?
                        < button className="button__basis">
                            <span className="button__text">{text}</span>
                            {hasArrow ? <span className="button__arrow">arrow_forward</span> : null}

                        </button>

                        : buttonType === "submit" ?
                            <label className="button__basis">
                                <input className="button__submit" type="submit"></input>
                                <span className="button__text">{text}</span>
                                {hasArrow ? <span className="button__arrow">arrow_forward</span> : null}
                            </label>
                            : null
            }
        </div >
    );
}

export default Button;