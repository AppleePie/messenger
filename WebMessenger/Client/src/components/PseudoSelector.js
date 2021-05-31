import React, {useEffect, useState} from 'react';


function PseudoSelector(props){
    const defaultImage = "/icons/default-photo.jpg";


    const handleClick = async (user) => {
        props.setIsHidden(true);
        props.users.delete(user.login);

        const response = await fetch('/api/chats', {
            method: 'POST',
            body: JSON.stringify({
                initiatorId:  props.currentuser,
                interlocutorName: user.login
            }),
            headers: {
                'Content-Type': 'application/json'
            }
        }).then(r => r.json());

        const responseAvatar = await fetch(`api/users/${user.id}/avatar`);
        const avatar = responseAvatar.status === 404 ? defaultImage : await responseAvatar.text();
        const chatArr =props.chats.slice();
        chatArr.push({chatId:response, avatar:avatar,login:user.login, interlocutor:user.id})
        props.setChats(chatArr);
    }

    const renderUserButton = (user) => {
        return (
            <button className='found-user' key={user.id} onClick={() => handleClick(user)}>{user.login}</button>
        )
    };

    return(
        <div className='pseudoSelector'>
            {
                props.foundUsers.map(renderUserButton)
            }
        </div>
    )
}

export default PseudoSelector