import React, { useState,useEffect, useContext } from 'react';
import {
  Container, IconButton, Typography, Paper, Button, Rating, Avatar, TextField, Card, CardContent, CardHeader, Divider,
  Box, IconButton as MuiIconButton,Grid,CardMedia, CardActionArea
} from '@mui/material';
import { Save, ThumbUpAlt } from '@mui/icons-material';
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
  return (
    <div className='d-flex flex-row justify-content-start w-100' style={{
      marginTop:'120px'
    }}>
      <Container sx={{marginRight:'0px'}} >    
            <Paper elevation={3} sx={{ padding: 2 }} height={'500px'} position={'relative'}>
              <Typography variant="h5" gutterBottom>
                {video.title}
          </Typography>
          <div style={{ position: 'relative',overflow:'hidden'}}>
            <video style={{
              width: '100%', height:'500px'
          }}
                      controls={true}
            width="100%"
              src={videoContent(video.videoId)}
              onLoadedMetadata={handleLoadedMetadata} >
                  </video>
          </div> 

        <Box display="flex" alignItems="center" marginTop={1}>
            <Avatar sx={{ marginRight: 1 }} />
          
            <Link to={`/profile/${video.channelId}`}><Typography>{video.channelName}</Typography>
            </Link>
                    <Button variant="outlined" sx={{ marginLeft: '20px' }} onClick={handleSubscribe}>
                    {uiState.isSubscribed?'Unsubscribe':'Subscribe' }
                      </Button>
                  <Rating name="video-rating" value={uiState.myRating} onChange={ev=>handleRatingChange(ev.target.value)}
                                  sx={{marginLeft:'auto'}} />
                <IconButton>
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
            <CommentCard key={comment.id} comment={comment} />
          ))}
        </Paper>
      </Container>
      <div className='d-flex flex-column' style={{marginRight:'auto',width:'350px'}}>
      {playlist&&<PlaylistCard playlist={playlist} navigate={navigate}/>}
      <Grid item md={6} sx={{marginTop:'32px'}}>
        <Typography variant="h5" gutterBottom>
          Recommended Videos
        </Typography>
        {/* Map through recommended videos and render RecommendedVideoCard for each */}
        {recommendedVideos.map((video, index) => (
          <RecommendedVideoCard key={index} video={video} navigate={navigate}/>
        ))}
      </Grid>
      </div>
         
    </div>
  );
}

function CommentCard({ comment}) {
  return (
    <Card sx={{ marginTop: 2 }}>
      <CardHeader
        avatar={<Avatar src={comment.profilePicture } />}
        title={comment.username}
        subheader={formatRelativeTime(new Date(comment.timestamp))}
        action={<IconButton><ThumbUpAlt /></IconButton>}
      />
      <CardContent>
        <Typography>{comment.text}</Typography>
      </CardContent>
      <Divider />
    </Card>
  );
}

const RecommendedVideoCard = ({ video,selected,navigate}) => {
  return (
    <Card sx={{
      display: 'flex', width: '100%', marginBottom: '2px',
      boxShadow: 'none',filter:selected?'brightness(95%)':'none', cursor: 'pointer',

    }} onClick={ev=>navigate('/player/'+video.id)}>
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
          <div style={{ marginRight: '8px' }}>{video.poster}</div>
          <div>{video.views}</div>
        </Typography>
      </CardContent>
      </CardActionArea>
   
    </Card>
  );
};


const PlaylistCard = ({ playlist,navigate}) => {
  const {playlistName, playlistOwner, currentVideoIndex, totalVideos, videos }=playlist
  return (
    <Card sx={{ maxHeight: '650px'}}>
      <CardContent sx={{display:'flex',flexDirection:'column',padding:'0px'}}>
        {/* Playlist Name */}
        <div className="px-3 my-2">
          <Typography variant="h5" gutterBottom>
            {playlistName}
          </Typography>
          {/* Playlist Owner */}
          <Typography variant="caption" color="textSecondary" gutterBottom>
            {`By ${playlistOwner}`}
          </Typography>
          {/* Video Index / Total Videos */}
          <Typography variant="caption" color="textSecondary">
            {`Video ${currentVideoIndex} / ${totalVideos}`}
          </Typography>
        </div>
       
        {/* Videos List */}
        <div style={{ overflowY: 'scroll',maxHeight:'500px'}}>
          {/* Map through playlist videos and render them */}
          {videos.map((video, index) => (
            <RecommendedVideoCard key={index} video={video} selected={index + 1 == currentVideoIndex} navigate={ navigate} />
          ))}
        </div>
      </CardContent>
    </Card>
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
  /* const recommendedVideos = new Array(20).fill({
    title: 'Title', thumbnailUrl: "https://via.placeholder.com/345x200"
    , poster: 'someDude', views: '50k'
  })
  const returnValue={recommendedVideos}
  if (params.playlistId)
  {
    //fetchPlaylist
    const playlist = {
      playlistName: "name", playlistOwner: 'me', currentVideoIndex: 1, totalVideos: 20,
      videos:recommendedVideos
    }
    returnValue['playlist']=playlist
  }
  else
    returnValue['playlist']=null
    return returnValue */
}


