import React, {useState} from 'react';

function ChatWindow(props) {

    const [textMessage, setTextMessage] = useState('')


    const handleChange = (event) => {
        setTextMessage(event.target.value)
    }

    const postChat = async () => await fetch('/api/chats', {
        method: 'POST',
        body: JSON.stringify({
            initiatorId: props.currentUser,
            interlocutorName: props.currentInterlocutor.login
        }),
        headers: {
            'Content-Type': 'application/json'
        }
    });


    const sendMessage = async () => {
        await fetch('/Chat/messages', {
            method: 'POST',
            body: JSON.stringify({
                initiator: props.currentUser,
                interlocutor: props.currentInterlocutor.interlocutor,
                message: textMessage
            }),
            headers: {
                'Content-Type': 'application/json'
            }
        })
    }


    const handleSend = async (event) => {
        event.preventDefault();
        if(textMessage === ''){
            alert('Пустое сообщение');
        }
        if (props.currentChatId === '' ) {
            const response = await postChat().then(r => r.json());
            props.currentInterlocutor.scroll.delete(props.currentInterlocutor.login);
            props.setCurrentChatId(response);
            const currentChats = props.chatObj.slice();
            currentChats.push({
                chatId: response,
                avatar: props.currentInterlocutor.avatar,
                login: props.currentInterlocutor.login,
                interlocutor: props.currentInterlocutor.interlocutor
            })
            props.setChats(currentChats);
        }
        sendMessage();
    }

    const renderMessage = (message) => {
        return (
            <p className={message.initiator === props.currentUser ? 'my-message' : 'interlocutor-message'} key={message.message}>{message.message}</p>
        )
    }


    return (
        <div className='chat-window'>
            <div className='messages-wrapper'>
            </div>
            <div className='message-panel'>
                <div className='user-chat-wrapper'>
                    <img className='user-chat-avatar' src={props.currentUserAvatar} alt='avatar'/>
                    <p className='chat-under-avatar-text'>{props.currentUserLogin}</p>
                </div>
                <div className='input-message-wrapper'>
                    <textarea id='message' className='input-send-message' onChange={handleChange}/>
                    <label htmlFor='message' className='send-message-button' onClick={handleSend}>Send Message</label>
                </div>
                <div className='user-chat-wrapper'>
                    <img className='user-chat-avatar' src={props.currentInterlocutor.avatar} alt='avatar'/>
                    <p className='chat-under-avatar-text'>{props.currentInterlocutor.login}</p>
                </div>
            </div>
        </div>
    )
}

export default ChatWindow