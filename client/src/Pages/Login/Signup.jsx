import TextField from '@mui/material/TextField';
import { FormControl,FormControlLabel,RadioGroup,FormLabel,Radio } from '@mui/material';
import Button from '@mui/material/Button';
import Alert from '@mui/material/Alert'
import './Login.css'
import { Link, useNavigate } from 'react-router-dom';
import { userController } from '../../Constants';
import { CURRENT_USER } from '../../Constants';
import { useState } from 'react';

function loginFailed(setLoginState,msg)
{
    setLoginState((oldValue) =>
    {
        setTimeout(setLoginState({
            ...oldValue
        }),3000)
        return {
            ...oldValue,flag:!oldValue.flag,msg
        }
    })
}

export function loginUser(signUpState,navigate)
{
    localStorage.setItem(CURRENT_USER,JSON.stringify(signUpState))
    navigate('/home')
}

async function signup(signUpState,navigate,setLoginState)
{
    const response = await fetch(userController + '/AddUser', {
        method:'POST',
        headers: {
            "Content-Type":"application/json"
        },
        body: JSON.stringify(signUpState)
    })

    if (response.status === 200)
    {
        const userId = await response.text()
        const response2 = await fetch(userController + `/LogIn/${signUpState.username}/${signUpState.password}`,
            {
        })
        if (response2.ok)
        {
            const jsonResponse = await response2.json() 
            const user = jsonResponse.item1
            user.sessionToken = jsonResponse.item2
            loginUser(user,navigate)
        }
        else
        {
            loginFailed(setLoginState,await response.text())
            }

    }
    else
    {
        loginFailed(setLoginState,await response.text())
    }
}

export default function Signup() { 

    const [signupFailedState, setSignupFailedState] = useState({flag:false,msg:''})
    const navigate = useNavigate()
    const [signupState,setSignupState] = useState({
        username:"",password:"",email:"",bio:"",thumbnail:"",name:"",age:0,gender:'male'
    })

    function onChange(event)
    {
        setSignupState((oldValue) =>
        {
            return {
                ...oldValue,
                [event.target.name]:event.target.value
            }
        })
    }

    function onFileUpload(event)
    {
        const file = event.target.files[0]
        if (file == null) return
        const fileReader = new FileReader()
        fileReader.readAsDataURL(file)
        fileReader.onload=(ev) =>
        {
            alert("Image uploaded.")
            setSignupState(oldValue => {
                return {
                    ...oldValue,
                    thumbnail: fileReader.result
                }
            })
        }
    }

    return <div className="login_container">
        <h2>Signup</h2>
        <TextField id="Name" label="Name" variant="outlined" sx={{
            marginTop:"10px",
            marginBottom: "10px"
            
        }}
            value={signupState.name}
            name='name'
        onChange={(onChange)}/>
        <TextField id="Username" label="Username" variant="outlined" sx={{
            marginBottom: "10px"
            
        }}
            value={signupState.username}
            name='username'
        onChange={(onChange)}/>
        <TextField id="Password" label="Password" variant="outlined" type='password'
            sx={{
                marginBottom:"10px"
            }}
            value={signupState.password}
            name='password'
            onChange={(onChange)} />
        <TextField id="Email" label="Email" variant="outlined" type='email'
            sx={{
                marginBottom:"10px"
            }}
            value={signupState.email}
            name='email'
            onChange={(onChange)} />
        <TextField id="Bio" label="Bio" variant="outlined" type='text'
            sx={{
                marginBottom:"10px"
            }}
            value={signupState.bio}
            name='bio'
            onChange={(onChange)} />
        <TextField
            id='Age'
          label="Age"
          variant="outlined"
            type="number"
            name='age'
          value={signupState.age}
            onChange={(onChange)}
            sx={{
                marginBottom:"10px"
            }}
        />
         <FormControl component="fieldset" margin="normal" fullWidth sx={{width:'220px'}}>
          <FormLabel component="legend">Gender</FormLabel>
            <RadioGroup
                row
            aria-label="gender"
            name="gender"
            value={signupState.gender}
            onChange={onChange}
                >
            <FormControlLabel value="male" control={<Radio />} label="Male" style={{color:'gray'} } />
            <FormControlLabel value="female" control={<Radio />} label="Female"  style={{color:'gray'} }/>
          </RadioGroup>
        </FormControl>
        <label htmlFor='input_file'>Add profile picture.</label> <input id='input_file' type='file' onChange={onFileUpload}
        accept=".jpg, .jpeg, .png" ></input>
        {signupFailedState.flag && <Alert severity="error">Signup failed. Reason: {signupFailedState.msg}</Alert>}
        <Button variant="text" sx={{
            width:"80px"
        }} id='Button' onClick={(ev) =>
        {
            signup(signupState, navigate, setSignupFailedState)
        }}>OK</Button>
        <Link to='/login'>Already have an account? Log in.</Link>
       
    </div>
}
