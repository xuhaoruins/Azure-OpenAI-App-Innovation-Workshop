import React from 'react';
import logo from "./logo.png"
import './App.css';
import Chat, { Bubble, Avatar, MessageProps, useMessages } from '@chatui/core';
import '@chatui/core/dist/index.css';

function App(props: any) {
    document.title = props.aiName;
    const { messages, appendMsg, setTyping, updateMsg } = useMessages([]);

    function uuidv5(len: number, radix: number | undefined = undefined) {
        var chars =
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".split("");
        var uuid = [],
            i;
        radix = radix || chars.length;

        if (len) {
            for (i = 0; i < len; i++) uuid[i] = chars[0 | (Math.random() * radix)];
        } else {
            var r;
            uuid[8] = uuid[13] = uuid[18] = uuid[23] = "-";
            uuid[14] = "4";
            for (i = 0; i < 36; i++) {
                if (!uuid[i]) {
                    r = 0 | (Math.random() * 16);
                    uuid[i] = chars[i == 19 ? (r & 0x3) | 0x8 : r];
                }
            }
        }
        return uuid.join("");
    }

    async function handleSend(type: string, val: string) {
        if (type === 'text' && val.trim()) {
            appendMsg({
                type: 'text',
                content: { text: val },
                position: 'right',
            });
            setTyping(true);

            prompt(val);
        }
    }

    const prompt = (val: string) => {
        fetch(
            `${props.apiBaseUrl}/engine/prompt?userid=${uuidv5(8)}&prompt=${val}`,
            {
                headers: {
                    "Content-Type": "application/json",
                },
                method: "get",
            }
        ).then(res => {
            return res.json()
        }).then(data => {
            appendMsg({
                type: "text",
                content: { text: data.completion },
            });
        }).catch(err => {
            console.log(err)
        })
    }

    function renderMessageContent(msg: MessageProps) {
        const { content } = msg;
        return (
            <div style={{ display: "flex" }}>
                {
                    msg.position === "left" &&
                    <div style={{ flexShrink: 0 }}>
                        <Avatar src={logo} size="lg" />
                    </div>
                }
                <div style={{ display: "flex", flexDirection: "column", marginLeft: "8px" }}>
                    {msg.position === "left" && <span style={{ fontSize: ".8rem", marginBottom: "2px", color: "#8C8C8C" }}>{props.aiName}</span>}
                    <Bubble content={content.text} />
                </div>
            </div>
        );
    }

    return (
        <Chat
            messages={messages}
            renderMessageContent={renderMessageContent}
            onSend={handleSend}
        />
    );
}

export default App;
