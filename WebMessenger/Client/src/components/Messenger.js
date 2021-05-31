import React, {useEffect, useState} from 'react';
import Chats from "./Chats";
import ChatWindow from "./ChatWindow";
import {useHistory} from "react-router-dom";


function Messenger(props) {
    const userUrl = '/api/users';
    const defaultImage = "/icons/default-photo.jpg";

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

    const getUserAvatar = async (id) => {
        const response = await fetch(`${userUrl}/${id}/avatar`);
        return response.status === 404 ? defaultImage : await response.text();
    }

    const fetchChats = async (id) => {
        const chats = (await getUser(id)).chats;
        const res = [];
        for (const chat of chats) {
            const result = (await getUser(chat.interlocutor)).login;
            const avatar = await getUserAvatar(chat.interlocutor);
            const chatId = chat.id;
            res.push({chatId: chatId, avatar: avatar, login: result, interlocutor: chat.interlocutor});
        }
        return res;
    }


    useEffect(() => {
        let isMounted = true;
        if (props.userId === '') {
            history.push('/login')
            return;
        }
        fetchChats(props.userId).then(r => {
            setChats(r);
            setLoading(false);
        });
        return () => {
            isMounted = false
        };
    }, []);


    return (
        <div className='messenger-wrapper'>
            <Header/>
            <div className='chats-wrapper'>
                <Chats chats={isLoading ? [] : chatObj} currentUser={props.userId} setChats={setChats}/>
            </div>
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