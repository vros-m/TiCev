import React, { useState,useEffect, useContext } from 'react';
import {
  Container, IconButton, Typography, Paper, Button, Rating, Avatar, TextField, Card, CardContent, CardHeader, Divider,
  Box, IconButton as MuiIconButton,Grid,CardMedia, CardActionArea,Dialog,DialogTitle,DialogContent
} from '@mui/material';
import { Save, ThumbUpAlt, DeleteOutline as DeleteIcon } from '@mui/icons-material';
import AddIcon from '@mui/icons-material/Add';
import {useLoaderData,Link, useNavigate} from 'react-router-dom'
import { CURRENT_USER, formatRelativeTime, profilePicture, thumbnail, thumbnailByVideoId, userController, videoContent, videoController } from '../Constants';
import UserContext from './Contexts/UserContext';


function VideoPlayer() {

  const [uiState, setUiState] = useState({
    myRating:0,rating:0,isSubscribed:false,comments:[]
  })
  const [isDescriptionOpen, setDescriptionOpen] = useState(false);
  const [newComment, setNewComment] = useState('');
  const [userState,setUserState] = useContext(UserContext)

  const {video, recommendedVideos,playlist } = useLoaderData();

  useEffect(() =>
  {
    fetch(videoController + `/IncrementViews/${video.id}`, {
      method:'PUT'
    })
    setUiState(oldValue =>
    {
      return {
        ...oldValue,
        myRating: video.myRating,
        rating: video.rating,
        isSubscribed: video.isSubscribed,
        comments: video.comments,
        views:video.views+1
      }
      })
  }, [video])

  async function handleRatingChange(value)
  {
    
    const response = await fetch(videoController + `/RateVideo/${video.id}/${userState.id}/${value}`,
      {
      method:'PUT'
      })
    if (response.ok)
    {
      const newRating = Number.parseFloat(await response.text())
      setUiState(oldValue => {
        return {...oldValue,myRating:value,rating:newRating}
      })
      }

  }
  function handleDescriptionToggle() {
    setDescriptionOpen(!isDescriptionOpen);
  }

  async function handleCommentSubmit(replyingToId,replyingToUserId) {
    if (newComment.trim() !== '') {
      const comment = { userId: userState.id,
        videoId: video.id,
        text: newComment,
        videoPosterId:video.channelId}
      const response = await fetch(videoController + `/AddComment`, {
        method:'POST',
        headers: {
          'Content-Type':'application/json'
        },
        body: JSON.stringify(
         comment
        )
      })
      if (response.ok)
      {
        const commentItem = await response.json()
        commentItem['profilePicture'] = profilePicture(userState.id)
        commentItem['username']=userState.username
        setUiState(oldValue => {
          return {
            ...oldValue,
            comments:[commentItem,...oldValue.comments]
          }
        })
        setNewComment('')
      }
    }
  }

  async function handleSubscribe(ev)
  {
    const response = await fetch(userController + `/Subscribe/${!uiState.isSubscribed}`,
      {
        method: 'POST',
        headers: {
          'Content-Type':'application/json'
        },
        body: JSON.stringify({
          subscriberId: userState.id,
          channelId: video.channelId,
          channelName:video.channelName
        })
      }) 
    if (response.ok)
    {
      setUiState(oldValue => {
        return {...oldValue,isSubscribed:!oldValue.isSubscribed}
      })  
    }
  }
  const handleLoadedMetadata = (event) => {
    console.log('Video metadata loaded:', event.target.duration);
  };

  const navigate = useNavigate()
  const [playlistDialogOpen,setPlaylistDialog] = useState(false)
  return (
    <div className='d-flex flex-row justify-content-start w-100' style={{
      marginTop:'120px'
    }}>
      <AddToPlaylistDialog open={playlistDialogOpen} onClose={ev=>setPlaylistDialog(false)} videoId={video.id}/>
      <Container sx={{marginRight:'0px'}} >    
            <Paper elevation={3} sx={{ padding: 2 }} height={'500px'} position={'relative'}>
              <Typography variant="h5" gutterBottom>
                {video.title}
          </Typography>
          <div style={{ position: 'relative',overflow:'hidden'}}>
            <video style={{
              width: '100%', height:'500px'
          }}
              controls
              autoPlay
            width="100%"
              src={videoContent(video.videoId)}
              onLoadedMetadata={handleLoadedMetadata} >
                  </video>
          </div> 

        <Box display="flex" alignItems="center" marginTop={1}>
            <Avatar sx={{ marginRight: 1 }} />
          
            <Link to={`/profile/${video.channelId}`}><Typography>{video.channelName}</Typography>
            </Link>
            {video.channelId!=userState.id&&<Button variant="outlined" sx={{ marginLeft: '20px' }} onClick={handleSubscribe}>
              {uiState.isSubscribed ? 'Unsubscribe' : 'Subscribe'}
            </Button>}
                  <Rating name="video-rating" value={uiState.myRating} onChange={ev=>handleRatingChange(ev.target.value)}
                                  sx={{marginLeft:'auto'}} />
                <IconButton onClick={ev=>setPlaylistDialog(true)}>
                  <Save />
                </IconButton>
                
              </Box>
               
            </Paper>

        <Paper elevation={3} sx={{ padding: 2, marginTop: 2 }}>
              <Box display="flex" alignItems="center" marginBottom={2}>
                  <Typography variant="h6" >
                        Description
                      </Typography>
                      <Typography variant="h7" marginLeft={'auto'} >
                        {uiState.views} {uiState.views!=1?'views':'view'}
                    </Typography>
                    {/* Rating */}
                      <Rating value={uiState.rating} readOnly sx={{ marginLeft: '10px' }} />
                    <Typography sx={{ marginLeft: 1 }}>{uiState.rating.toFixed(1)}</Typography>
                </Box>
          <Typography>
            {video.description}
          </Typography>
          <MuiIconButton onClick={handleDescriptionToggle}>
            <i className={`bi bi-chevron-${isDescriptionOpen ? 'up' : 'down'}`} />
          </MuiIconButton>
        </Paper>
        {/* Comments */}
        <Paper elevation={3} sx={{ padding: 2, marginTop: 2 }}>
          <Typography variant="h6" gutterBottom>
            Comments ({uiState.comments.length})
          </Typography>
          <TextField
            label="Add a public comment..."
            variant="outlined"
            fullWidth
            value={newComment}
            onChange={(e) => setNewComment(e.target.value)}
          />
          <Button variant="contained" color="primary" onClick={handleCommentSubmit}>
            Comment
          </Button>
          {uiState.comments.map((comment) => (
            <CommentCard key={comment.id} comment={comment} setUiState={setUiState}/>
          ))}
        </Paper>
      </Container>
      <div className='d-flex flex-column' style={{marginRight:'auto',width:'350px'}}>
      {playlist&&<PlaylistCard playlist={playlist} currentVideoId={video.id}/>}
      <Grid item md={6} sx={{marginTop:'32px'}}>
        <Typography variant="h5" gutterBottom>
          Recommended Videos
        </Typography>
        {/* Map through recommended videos and render RecommendedVideoCard for each */}
        {recommendedVideos.map((video, index) => (
          <RecommendedVideoCard key={index} video={video}/>
        ))}
      </Grid>
      </div>
         
    </div>
  );
}

function CommentCard({ comment,setUiState }) {
  
  const [userState, setUserState] = useContext(UserContext)
  async function deleteComment(ev)
  {
    const request = await fetch(videoController + '/DeleteComment/' + comment.id,
      {
      method:'DELETE'
    })
    if (request.ok)
    {
      setUiState(oldValue =>
      {
        return {
          ...oldValue,
          comments:[...oldValue.comments.filter(c=>c.id!=comment.id)]
        }
        })
      }
  }
  return (
    <Card sx={{ marginTop: 2 }}>
      <CardHeader
        avatar={<Avatar src={comment.profilePicture } />}
        title={comment.username}
        subheader={formatRelativeTime(new Date(comment.timestamp))}
        action={userState.id==comment.userId&&<IconButton onClick={deleteComment}><DeleteIcon/></IconButton>}
      />
      <CardContent>
        <Typography>{comment.text}</Typography>
      </CardContent>
      <Divider />
    </Card>
  );
}

const RecommendedVideoCard = ({ video, selected, playlistId }) => {
  const navigate = useNavigate()
  return (
    <Card sx={{
      display: 'flex', width: '100%', marginBottom: '2px',
      boxShadow: 'none',filter:selected?'brightness(95%)':'none', cursor: 'pointer',

    }} onClick={ev=>navigate(`/player/${video.id}${playlistId?`/${playlistId}`:''}`)}>
      <CardActionArea sx={{
        display: 'flex', width: '100%', 
        boxShadow: 'none',  cursor: 'pointer',
      }}>
       {/* Thumbnail */}
       <CardMedia
        component="img"
        alt={video.title}
        height="100"
        sx={{
          maxWidth:"150px"
        }}
        image={thumbnailByVideoId(video.id)}
      />
      {/* Content */}
      <CardContent sx={{ flex: 1,padding:'8px',paddingTop:'0px'}}>
        {/* Title */}
        <Typography variant="h6" component="div">
          {video.title}
        </Typography>
        {/* Poster and Views */}
        <Typography variant="body2" color="text.secondary" sx={{
          display: 'flex', alignItems: 'start',
          flexDirection: 'column'
        }}>
          <div style={{ marginRight: '8px' }}>{video.channelName}</div>
            <div>{video.views} {video.views!=1?'views':'view' }</div>
        </Typography>
      </CardContent>
      </CardActionArea>
   
    </Card>
  );
};


const PlaylistCard = ({ playlist,currentVideoId}) => {
  const { title, channelName, channelId, videos,id } = playlist
  const [currentVideoIndex, setCurrentVideoIndex] = useState(0)

  useEffect(() => {
    setCurrentVideoIndex(  videos.findIndex(
      video=>video.id==currentVideoId
    ))
  },[currentVideoId])


  const totalVideos=videos.length
  return (
    <Card sx={{ maxHeight: '650px'}}>
      <CardContent sx={{display:'flex',flexDirection:'column',padding:'0px'}}>
        {/* Playlist Name */}
        <div className="px-3 my-2">
          <Typography variant="h5" gutterBottom>
            {title}
          </Typography>
          {/* Playlist Owner */}
          <div className='w-100 d-flex flex-row justify-content-between'>
            <Typography variant="caption" color="textSecondary" gutterBottom>
              {`By ${channelName}`}
            </Typography>
            {/* Video Index / Total Videos */}
            <Typography variant="caption" color="textSecondary">
              {`Video ${currentVideoIndex+1} / ${totalVideos}`}
            </Typography>
          </div>
        </div>
       
        {/* Videos List */}
        <div style={{ overflowY: 'scroll',maxHeight:'500px'}}>
          {/* Map through playlist videos and render them */}
          {videos.map((video, index) => (
            <RecommendedVideoCard key={index} video={video} selected={video.id===currentVideoId} playlistId={id} />
          ))}
        </div>
      </CardContent>
    </Card>
  );
};


const AddToPlaylistDialog = ({ open, onClose,videoId }) => {
  const [creatingPlaylist, setCreatingPlaylist] = useState(false);
  const [newPlaylistName, setNewPlaylistName] = useState('');
  const [playlists, setPlaylists] = useState([])
  const [userState, setUserState] = useContext(UserContext)
  const myOnClose = () => {
    setCreatingPlaylist(false)
    onClose()
  }
  useEffect(() => {
    fetch(userController + `/GetPlaylists/${userState.id}`).then(r =>
    {
      if (r.ok)
      {
        r.json().then(result=>setPlaylists(result))
      }
      else throw new Error("Can't fetch playlists.")
      })
  },[])
  const handlePlaylistClick = async (playlistId,index) => {
    const response = await fetch(userController + `/AddVideoToPlaylist/${playlistId}/${videoId}`,
      {
      method:'POST'
    })
    if (response.ok)
    {
      playlists[index].videoIds.push(videoId)
      setPlaylists(oldValue=>[...oldValue])
    }
    else alert("error!")
   
    //myOnClose();
  };

  const handleCreatePlaylist = async () => {
    // Add logic to handle creating a new playlist
    const response = await fetch(userController + `/CreatePlaylist`, {
      method: 'POST',
      headers: {
        "Content-Type":'application/json'
      },
      body: JSON.stringify(
        {
          title: newPlaylistName,
          channelId: userState.id,
          channelName: userState.username,
          videoIds:[videoId]
        }
      )
    })
    if (response.ok)
    {
      const playlist = await response.json()
      setPlaylists(oldValue => {
        return [...oldValue,playlist]
      })
      myOnClose()
    }
    else (
      alert("Error!")
    )
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Add to Playlist</DialogTitle>
      <DialogContent>
        {!creatingPlaylist ? (
          <>
            {playlists.map((playlist,index) => (
              <Card key={playlist.id} sx={{ marginBottom: 2 }}>
                <CardContent style={{ display: 'flex', alignItems: 'center', justifyContent:'space-between'}}>
                  <div style={{ display: 'flex', alignItems: 'center', justifyContent:'space-between'}}><img marginRight={16} width={50} height={50} src={playlist.videoIds.length!=0?thumbnailByVideoId(
                    playlist.videoIds[0]):''} />
                  <Typography style={{marginLeft:10}} variant="subtitle1">{playlist.title}</Typography></div>
                  <IconButton
                    edge="end"
                    aria-label="add"
                    onClick={() => handlePlaylistClick(playlist.id,index)}
                    disabled={playlist.videoIds.filter(vId=>vId==videoId).length!=0}
                  >
                    <AddIcon />
                  </IconButton>
                </CardContent>
              </Card>
            ))}
            <Button onClick={() => setCreatingPlaylist(true)}>Create Playlist</Button>
          </>
        ) : (
          <>
            <TextField
              label="Playlist Name"
              value={newPlaylistName}
              onChange={(e) => setNewPlaylistName(e.target.value)}
              fullWidth
              sx={{ marginBottom: 2,marginTop:2 }}
            />
            <Button onClick={handleCreatePlaylist}>Add Playlist</Button>
          </>
        )}
      </DialogContent>
    </Dialog>
  );
};

export default VideoPlayer;

export async function VideoLoader({params})
{
  const currentUser = JSON.parse(localStorage.getItem(CURRENT_USER))
  const videoResponse = await fetch(videoController + `/GetVideo/${params.videoId}/${currentUser.id}`)
  if (videoResponse.ok)
  {
    const video = await videoResponse.json()
    const result = {video}
    if (params.playlistId != undefined)
    {
      const playlistResponse = await fetch(userController + `/GetPlaylistContent/${params.playlistId}`)
      if (playlistResponse.ok)
      {
        const playlist = await playlistResponse.json()
        result.playlist=playlist
      }
      else
        result.playlist=null
    }
    const recommendedResponse = await fetch(videoController + `/GetRecommendedVideos/${video.id}`)
    if (recommendedResponse.ok)
    {
      const recommendedVideos = await recommendedResponse.json()
      result.recommendedVideos=recommendedVideos
    }
    return result
  }
  else
    throw error('Error loading video.')
}


