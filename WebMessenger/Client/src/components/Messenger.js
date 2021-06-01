import React, {useCallback, useEffect, useState} from 'react';
import Chats from "./Chats";
import ChatWindow from "./ChatWindow";
import {useHistory} from "react-router-dom";
import {HubConnectionBuilder} from "@microsoft/signalr";
import AccountManager from "./AccountManager";


function Messenger(props) {
    const userUrl = '/api/users';
    const defaultImage = "/icons/default-photo.jpg";

    const [chatObj, setChats] = useState({});
    const [isLoading, setLoading] = useState(true);
    const [currentUserAvatar, setCurrentUserAvatar] = useState('');
    const [currentUserLogin, setCurrentUserLogin] = useState('');
    const [currentChatId, setCurrentChatId] = useState('');
    const [currentInterlocutor, setCurrentInterlocutor] = useState({login: '', avatar: '', interlocutor: ''});
    const [currentMessages, setCurrentMessages] = useState([]);
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
        const res = {};
        for (const chat of chats) {
            const result = (await getUser(chat.interlocutor)).login;
            const avatar = await getUserAvatar(chat.interlocutor);
            const chatId = chat.id;
            res[chat.id] = {chatId: chatId, avatar: avatar, login: result, interlocutor: chat.interlocutor};
        }
        return res;
    }

    const pushMessage = useCallback(message => {
        const messages = currentMessages;
        messages.push(message);
        setCurrentMessages(messages.slice());
    }, [currentMessages])


    useEffect(() => {
        if (props.userId === '') {
            history.push('/login')
            return;
        }


        const connection = new HubConnectionBuilder()
            .withUrl('https://localhost:5001/hubs/chat')
            .withAutomaticReconnect()
            .build();

        connection.start()
            .then(_ => {
                connection.invoke("Send", props.userId).then(r => r);
            })
            .catch(e => e);

        connection.on('ReceiveMessage',pushMessage);

        getUser(props.userId).then(r => {
            setCurrentUserLogin(r.login);
        });
        getUserAvatar(props.userId).then(r => {
            setCurrentUserAvatar(r);
        });
        fetchChats(props.userId).then(r => {
            setChats(r);
            setLoading(false);
        });
    }, []);

    const renderChatWindow = () => {
        return !Object.values(currentInterlocutor).includes('') ? (
            <ChatWindow currentInterlocutor={currentInterlocutor}
                        currentUserAvatar={currentUserAvatar}
                        currentUserLogin={currentUserLogin}
                        currentChatId={currentChatId}
                        setCurrentChatId={setCurrentChatId}
                        currentUser={props.userId}
                        chatObj={chatObj}
                        setChats={setChats}
                        currentMessages={currentMessages}
                        setCurrentMessages={setCurrentMessages}
            />
        ) : (
            <div className='default-chat-window-message-wrapper'>
                <p className='default-chat-window-message'>Please select a chat to start messaging</p>
            </div>
        );

    }


    return (
        <div className='messenger-wrapper'>
            <Header userId={props.userId} login={currentUserLogin} avatar={currentUserAvatar} setLogin={setCurrentUserLogin}/>
            <div className='chats-wrapper'>
                <Chats chats={isLoading ? [] : chatObj}
                       setChats={setChats}
                       currentUser={props.userId}
                       setCurrentInterlocutor={setCurrentInterlocutor}
                       setCurrentChatId={setCurrentChatId}
                       setCurrentMessages={setCurrentMessages}
                />
                {renderChatWindow()}
            </div>
        </div>
    )
}

function Header(props) {
    const [showAccountManager, setShowAccountManager] = useState(false);

    return (
        <>
            <div className='messenger-header'>
                <button className='header-button' onClick={() => setShowAccountManager(!showAccountManager)}>☰</button>
                <b className='messenger-title'>OOP Task5</b>
            </div>
            {showAccountManager ? <AccountManager userId={props.userId} login={props.login} setLogin={props.setLogin} avatar={props.avatar} /> : null}
        </>
    )
}


export default Messenger