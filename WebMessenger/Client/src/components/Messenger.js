import React, {useEffect, useState} from 'react';
import Chats from "./Chats";
import {useHistory} from "react-router-dom";


function Messenger(props) {
    const userUrl = '/api/users';

    const statId4 = 'c6af1fa7-73e9-4ef2-8913-aacec4d43276';
    const [chatObj, setChats] = useState([]);
    const [isLoading, setLoading] = useState(true);
    const history = useHistory();

    const getUser = async (id) =>
        await fetch(`${userUrl}/${id}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'accept/application/json'
            }
        }).then(r => r.json());

    const fetchChats = async (id) => {
        const chats = (await getUser(id)).chats;
        const res = [];
        for (const chat of chats) {
            const result = (await getUser(chat.interlocutor)).login;
            const avatar = await fetch(`${userUrl}/${props.userId}/avatar`).then(r => r.text());
            const chatId = chat.id;
            res.push({chatId: chatId, avatar: avatar, login: result, interlocutor: chat.interlocutor});
        }
        setChats(res);
        setLoading(false);
    }


    useEffect(() => {
        if (props.userId === '') {
            history.push('/login')
            return;
        }
        fetchChats(props.userId);
    }, []);



    const renderChat = () => {
        return isLoading ? [] : chatObj;
    }


    return (
        <div className='messenger-wrapper'>
            <Header/>
            <Chats chats={renderChat()}/>
        </div>
    )
}

function Header() {
    return (
        <div className='messenger-header'>
            <button className='header-button'>☰</button>
            <b className='messenger-title'>OOP Task5</b>
        </div>
    )
}


export default Messenger