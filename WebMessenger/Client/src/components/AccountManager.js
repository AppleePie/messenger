import React, {useState} from 'react'
import {useHistory} from "react-router-dom";

export default function AccountManager(props) {
    const updateUserRoute = `/api/users/${props.userId}`;
    const postAvatar = '/avatar'
    const defaultLoginExceptionMessage = 'Empty User Name';
    const defaultInputNameClass = 'input-text';

    const [labelPasswordClass, setPasswordClass] = useState('label-text');
    const [labelUserNameClass, setUserNameClass] = useState('label-text');
    const [passwordException, setPasswordException] = useState('wrong-input');
    const [loginException, setLoginException] = useState('wrong-input');
    const [needMovePassword, setNeedMovePassword] = useState(true);
    const [needMoveUser, setNeedMoveUser] = useState(true);
    const [username, setUsername] = useState(props.login)
    const [preview, setPreview] = useState(props.avatar);
    const [file, setFile] = useState();
    const [userObj, setUserObj] = useState({login: '', password: ''});
    const [loginExceptionMessage, setLoginExceptionMessage] = useState(defaultLoginExceptionMessage);
    const [nameInputClass, setNameInputClass] = useState(defaultInputNameClass);
    const history = useHistory();

    const handlePasswordChange = (event) => {
        const value = event.target.value;
        setUserObj({...userObj, password: event.target.value});
        setNeedMovePassword(value === '');
    };

    const handleUserChange = (event) => {
        const value = event.target.value;
        setUserObj({...userObj, login: event.target.value});
        setNeedMoveUser(value === '');
    };

    const handleImageChange = (event) => {
        if (event.target.files[0]['type'].split('/')[0] === 'image') {
            const reader = new FileReader();
            const file = event.target.files[0];
            setFile(file);
            reader.onload = () => {
                setPreview(reader.result);
            }
            reader.readAsDataURL(file);
        } else {
            alert("You need to download image")
        }
    };

    const handleSubmit = async (event) => {
        event.preventDefault();

        const response = await fetch(updateUserRoute, {
            method: 'PUT',
            body: JSON.stringify(userObj),
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (response.status === 422) {
            setLoginExceptionMessage('For login allowed only alphabetic chars and digits');
            setLoginException('visible-wrong-input');
            setNameInputClass(`${defaultInputNameClass} bad-input`);
            return;
        }

        if (response.status === 409) {
            setLoginExceptionMessage('This name is taken');
            setLoginException('visible-wrong-input');
            setNameInputClass(`${defaultInputNameClass} bad-input`);
            return;
        }

        setUsername(userObj.login);
        props.setLogin(userObj.login);

        if (file) {
            const dataForResponse = new FormData();
            dataForResponse.append('uploads', file);
            await fetch(`${updateUserRoute}/${postAvatar}`, {
                method: 'POST',
                body: dataForResponse
            });
        }
    }


    return (
        <div className="account-manager-wrapper">
            <div className="form-account">
                <form className="form" method="POST" onSubmit={handleSubmit}>
                    <h3 className="sign-in-text">{username}</h3>
                    <div>
                        <input className="input-file" id="image" type="file" autoComplete="off"
                               onChange={handleImageChange}/>
                        <label htmlFor="image">
                            <img className="preview" src={preview} alt="preview"/>
                        </label>
                    </div>
                    <div>
                        <label className={labelUserNameClass} htmlFor="user-name">Change user name...</label>
                        <input className={nameInputClass} type="text" id="user-name" autoComplete="off"
                               onFocus={() => {
                                   setNameInputClass(defaultInputNameClass);
                                   setLoginExceptionMessage(defaultLoginExceptionMessage);
                                   setUserNameClass('label-text input-focused');
                                   setLoginException('wrong-input');
                               }}
                               onChange={handleUserChange}
                               onBlur={() => {
                                   if (needMoveUser) setUserNameClass('label-text')
                               }}/>
                        <p className={loginException}>{loginExceptionMessage}</p>
                    </div>
                    <div>
                        <label className={labelPasswordClass} htmlFor="password">Password</label>
                        <input className="input-text" type="text" id="password" autoComplete="off"
                               onFocus={() => {
                                   setPasswordClass('label-text input-focused')
                                   setPasswordException('wrong-input');
                               }}
                               onChange={handlePasswordChange}
                               onBlur={() => {
                                   if (needMovePassword) setPasswordClass('label-text');
                               }}/>
                        <p className={passwordException}>Empty password</p>
                    </div>
                    <input type="submit" className="submit-form" value="Submit"/>
                </form>
            </div>
        </div>
    );
}