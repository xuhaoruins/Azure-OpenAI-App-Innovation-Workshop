import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';

const root = ReactDOM.createRoot(
    document.getElementById('root') as HTMLElement
);

if (process.env.NODE_ENV === "production") {
    fetch("/settings", {
        headers: {
            "Content-Type": "application/json",
        },
        method: "get",
    }
    ).then(res => {
        return res.json()
    }).then(data => {
        root.render(
            <React.StrictMode>
                <App aiName={data.aiName} apiBaseUrl={data.apiBaseUrl} />
            </React.StrictMode>
        );
    }).catch(err => {
        console.log(err)
    })
} else {
    root.render(
        <React.StrictMode>
            <App aiName="智能助理" apiBaseUrl="https://localhost:44309/api"/>
        </React.StrictMode>
    );
}
// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
