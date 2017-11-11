using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class KDTree : MonoBehaviour{

    private struct ObjectNode{
        internal int begin;
        internal int end;
        internal int left;
        internal int right;
        internal float max_x;
        internal float max_z;
        internal float min_x;
        internal float min_z;
    }

    private struct FloatPair{
        private float a;
        private float b;

        internal FloatPair(float a1, float b1){
            a = a1;
            b = b1;
        }
        public static bool operator <(FloatPair pair1, FloatPair pair2){
            return pair1.a < pair2.a || !(pair2.a < pair1.a) && pair1.b < pair2.b;
        }

        public static bool operator <=(FloatPair pair1, FloatPair pair2){
            return (pair1.a == pair2.a && pair1.b == pair2.b) || pair1 < pair2;
        }

        public static bool operator >(FloatPair pair1, FloatPair pair2){
            return !(pair1 <= pair2);
        }

        public static bool operator >=(FloatPair pair1, FloatPair pair2){
            return !(pair1 < pair2);
        }
    }

    private const int MAX_LEAF_SIZE = 10;

	private RVOMove[] objects;
    private ObjectNode[] objectTree;

	void Update ()
	{
		buildTree ();
	}

    internal void buildTree(){
		RVOMove[] inScene = (RVOMove[]) GameObject.FindObjectsOfType (typeof (RVOMove));

		//Debug.Log ("num of rmos : " + inScene.Count ());

		if(objects == null || objects.Length != inScene.Count()){
			objects = new RVOMove[inScene.Count()];

            for(int i = 0; i < objects.Length; ++i){
                objects[i] = inScene[i];
            }

            objectTree = new ObjectNode[2 * objects.Length];

            for (int i = 0; i < objectTree.Length; ++i){
                    objectTree[i] = new ObjectNode();
            }
        }

        if(objects.Length != 0){
            buildObjectTreeRecursive(0, objects.Length, 0);
        }
    }
    
    private void buildObjectTreeRecursive(int begin, int end, int node){
        objectTree[node].begin = begin;
        objectTree[node].end = end;
        objectTree[node].min_x = objectTree[node].max_x = objects[begin].pos_a.x;
        objectTree[node].min_z = objectTree[node].max_z = objects[begin].pos_a.z;

        for(int i = begin + 1; i < end; ++i){
            objectTree[node].max_x = Math.Max(objectTree[node].max_x, objects[i].pos_a.x);
            objectTree[node].min_x = Math.Min(objectTree[node].min_x, objects[i].pos_a.x);
            objectTree[node].max_z = Math.Max(objectTree[node].max_z, objects[i].pos_a.z);
            objectTree[node].min_z = Math.Min(objectTree[node].min_z, objects[i].pos_a.z);
        }

        if(end - begin > MAX_LEAF_SIZE){
            bool isVertical = objectTree[node].max_x - objectTree[node].min_x > objectTree[node].max_z - objectTree[node].min_z;
            float splitValue = 0.5f * (isVertical ? objectTree[node].max_x + objectTree[node].min_x : objectTree[node].max_z + objectTree[node].min_z);

            int left = begin;
            int right = end;

            while(left < right){
                while(left < right && (isVertical ? objects[left].pos_a.x : objects[left].pos_a.z) < splitValue){
					++left;
                }

                while(right > left && (isVertical ? objects[right - 1].pos_a.x : objects[right-1].pos_a.z) >= splitValue){
					--right;
                }

                if(left < right){
                    RVOMove temp = objects[left];
                    objects[left] = objects[right - 1];
                    objects[right - 1] = temp;
                    ++left;
                    --right;
                }
            }
			int leftSize = left - begin;

			if(leftSize == 0){
				++leftSize;
				++left;
				++right;
			}
			objectTree[node].left = node + 1;
			objectTree[node].right = node + 2 * leftSize;

			buildObjectTreeRecursive(begin,left,objectTree[node].left);
			buildObjectTreeRecursive(left, end, objectTree[node].right);
        }  
    }
		
	private void queryObjectTreeRecursive(RVOMove o, ref float rangeSq, int node){
        if(objectTree[node].end - objectTree[node].begin <= MAX_LEAF_SIZE){
            for(int i = objectTree[node].begin; i < objectTree[node].end; ++i){
				o.insertObjectNeighbor(objects[i], ref rangeSq);
            }
        }else{
            float distSqLeft = RVOMath.sqr(Math.Max(0.0f, objectTree[objectTree[node].left].min_x - o.pos_a.x)) + RVOMath.sqr(Math.Max(0.0f, o.pos_a.x - objectTree[objectTree[node].left].max_x)) + RVOMath.sqr(Math.Max(0.0f, objectTree[objectTree[node].left].min_z - o.pos_a.z)) + RVOMath.sqr(Math.Max(0.0f, o.pos_a.z - objectTree[objectTree[node].left].max_z));
            float distSqRight = RVOMath.sqr(Math.Max(0.0f, objectTree[objectTree[node].right].min_x - o.pos_a.x)) + RVOMath.sqr(Math.Max(0.0f, o.pos_a.x - objectTree[objectTree[node].right].max_x)) + RVOMath.sqr(Math.Max(0.0f, objectTree[objectTree[node].right].min_z - o.pos_a.z)) + RVOMath.sqr(Math.Max(0.0f, o.pos_a.z - objectTree[objectTree[node].right].max_z));

            if(distSqLeft < distSqRight){
                if(distSqLeft < rangeSq){
                    queryObjectTreeRecursive(o, ref rangeSq, objectTree[node].left);
                    if(distSqRight < rangeSq){
                        queryObjectTreeRecursive(o, ref rangeSq, objectTree[node].right);
                    }
                }
            }else{
                if(distSqRight < rangeSq){
                    queryObjectTreeRecursive(o, ref rangeSq, objectTree[node].right);
                    if(distSqLeft < rangeSq){
                        queryObjectTreeRecursive(o, ref rangeSq, objectTree[node].left);
                    }
                }
            }
        }
    }

    internal void computeObjectNeighbors(RVOMove o, ref float rangeSq){
        queryObjectTreeRecursive(o, ref rangeSq, 0);
    }





//obstacle shit

 //might need later for static obstacles
    // private class ObstacleTreeNode
    //     {
    //         internal Obstacle obstacle_;
    //         internal ObstacleTreeNode left_;
    //         internal ObstacleTreeNode right_;
    //     };
//private ObstacleTreeNode obstacleTree;
// internal void buildObstacleTree()
    //     {
    //         obstacleTree_ = new ObstacleTreeNode();

    //         IList<Obstacle> obstacles = new List<Obstacle>(Simulator.Instance.obstacles_.Count);

    //         for (int i = 0; i < Simulator.Instance.obstacles_.Count; ++i)
    //         {
    //             obstacles.Add(Simulator.Instance.obstacles_[i]);
    //         }

    //         obstacleTree_ = buildObstacleTreeRecursive(obstacles);
    //     }

    // private ObstacleTreeNode buildObstacleTreeRecursive(IList<Obstacle> obstacles)
    //     {
    //         if (obstacles.Count == 0)
    //         {
    //             return null;
    //         }

    //         ObstacleTreeNode node = new ObstacleTreeNode();

    //         int optimalSplit = 0;
    //         int minLeft = obstacles.Count;
    //         int minRight = obstacles.Count;

    //         for (int i = 0; i < obstacles.Count; ++i)
    //         {
    //             int leftSize = 0;
    //             int rightSize = 0;

    //             Obstacle obstacleI1 = obstacles[i];
    //             Obstacle obstacleI2 = obstacleI1.next_;

    //             /* Compute optimal split node. */
    //             for (int j = 0; j < obstacles.Count; ++j)
    //             {
    //                 if (i == j)
    //                 {
    //                     continue;
    //                 }

    //                 Obstacle obstacleJ1 = obstacles[j];
    //                 Obstacle obstacleJ2 = obstacleJ1.next_;

    //                 float j1LeftOfI = RVOMath.leftOf(obstacleI1.point_, obstacleI2.point_, obstacleJ1.point_);
    //                 float j2LeftOfI = RVOMath.leftOf(obstacleI1.point_, obstacleI2.point_, obstacleJ2.point_);

    //                 if (j1LeftOfI >= -RVOMath.RVO_EPSILON && j2LeftOfI >= -RVOMath.RVO_EPSILON)
    //                 {
    //                     ++leftSize;
    //                 }
    //                 else if (j1LeftOfI <= RVOMath.RVO_EPSILON && j2LeftOfI <= RVOMath.RVO_EPSILON)
    //                 {
    //                     ++rightSize;
    //                 }
    //                 else
    //                 {
    //                     ++leftSize;
    //                     ++rightSize;
    //                 }

    //                 if (new FloatPair(Math.Max(leftSize, rightSize), Math.Min(leftSize, rightSize)) >= new FloatPair(Math.Max(minLeft, minRight), Math.Min(minLeft, minRight)))
    //                 {
    //                     break;
    //                 }
    //             }

    //             if (new FloatPair(Math.Max(leftSize, rightSize), Math.Min(leftSize, rightSize)) < new FloatPair(Math.Max(minLeft, minRight), Math.Min(minLeft, minRight)))
    //             {
    //                 minLeft = leftSize;
    //                 minRight = rightSize;
    //                 optimalSplit = i;
    //             }
    //         }

    //         {
    //             /* Build split node. */
    //             IList<Obstacle> leftObstacles = new List<Obstacle>(minLeft);

    //             for (int n = 0; n < minLeft; ++n)
    //             {
    //                 leftObstacles.Add(null);
    //             }

    //             IList<Obstacle> rightObstacles = new List<Obstacle>(minRight);

    //             for (int n = 0; n < minRight; ++n)
    //             {
    //                 rightObstacles.Add(null);
    //             }

    // //             int leftCounter = 0;
    // //             int rightCounter = 0;
    // //             int i = optimalSplit;

    // //             Obstacle obstacleI1 = obstacles[i];
    // //             Obstacle obstacleI2 = obstacleI1.next_;

    // //             for (int j = 0; j < obstacles.Count; ++j)
    // //             {
    // //                 if (i == j)
    // //                 {
    // //                     continue;
    // //                 }

    // //                 Obstacle obstacleJ1 = obstacles[j];
    // //                 Obstacle obstacleJ2 = obstacleJ1.next_;

    // //                 float j1LeftOfI = RVOMath.leftOf(obstacleI1.point_, obstacleI2.point_, obstacleJ1.point_);
    // //                 float j2LeftOfI = RVOMath.leftOf(obstacleI1.point_, obstacleI2.point_, obstacleJ2.point_);

    // //                 if (j1LeftOfI >= -RVOMath.RVO_EPSILON && j2LeftOfI >= -RVOMath.RVO_EPSILON)
    // //                 {
    // //                     leftObstacles[leftCounter++] = obstacles[j];
    // //                 }
    // //                 else if (j1LeftOfI <= RVOMath.RVO_EPSILON && j2LeftOfI <= RVOMath.RVO_EPSILON)
    // //                 {
    // //                     rightObstacles[rightCounter++] = obstacles[j];
    // //                 }
    // //                 else
    // //                 {
    // //                     /* Split obstacle j. */
    // //                     float t = RVOMath.det(obstacleI2.point_ - obstacleI1.point_, obstacleJ1.point_ - obstacleI1.point_) / RVOMath.det(obstacleI2.point_ - obstacleI1.point_, obstacleJ1.point_ - obstacleJ2.point_);

    // //                     Vector2 splitPoint = obstacleJ1.point_ + t * (obstacleJ2.point_ - obstacleJ1.point_);

    // //                     Obstacle newObstacle = new Obstacle();
    // //                     newObstacle.point_ = splitPoint;
    // //                     newObstacle.previous_ = obstacleJ1;
    // //                     newObstacle.next_ = obstacleJ2;
    // //                     newObstacle.convex_ = true;
    // //                     newObstacle.direction_ = obstacleJ1.direction_;

    // //                     newObstacle.id_ = Simulator.Instance.obstacles_.Count;

    // //                     Simulator.Instance.obstacles_.Add(newObstacle);

    // //                     obstacleJ1.next_ = newObstacle;
    // //                     obstacleJ2.previous_ = newObstacle;

    // //                     if (j1LeftOfI > 0.0f)
    // //                     {
    // //                         leftObstacles[leftCounter++] = obstacleJ1;
    // //                         rightObstacles[rightCounter++] = newObstacle;
    // //                     }
    // //                     else
    // //                     {
    // //                         rightObstacles[rightCounter++] = obstacleJ1;
    // //                         leftObstacles[leftCounter++] = newObstacle;
    // //                     }
    // //                 }
    // //             }

    // //             node.obstacle_ = obstacleI1;
    // //             node.left_ = buildObstacleTreeRecursive(leftObstacles);
    // //             node.right_ = buildObstacleTreeRecursive(rightObstacles);

    // //             return node;
    // //         }
    // //     }
    // // private void queryObstacleTreeRecursive(Agent agent, float rangeSq, ObstacleTreeNode node)
    // //     {
    // //         if (node != null)
    // //         {
    // //             Obstacle obstacle1 = node.obstacle_;
    // //             Obstacle obstacle2 = obstacle1.next_;

    // //             float agentLeftOfLine = RVOMath.leftOf(obstacle1.point_, obstacle2.point_, agent.position_);

    // //             queryObstacleTreeRecursive(agent, rangeSq, agentLeftOfLine >= 0.0f ? node.left_ : node.right_);

    // //             float distSqLine = RVOMath.sqr(agentLeftOfLine) / RVOMath.absSq(obstacle2.point_ - obstacle1.point_);

    // //             if (distSqLine < rangeSq)
    // //             {
    // //                 if (agentLeftOfLine < 0.0f)
    // //                 {
    // //                     /*
    // //                      * Try obstacle at this node only if agent is on right side of
    // //                      * obstacle (and can see obstacle).
    // //                      */
    // //                     agent.insertObstacleNeighbor(node.obstacle_, rangeSq);
    // //                 }

    // //                 /* Try other side of line. */
    // //                 queryObstacleTreeRecursive(agent, rangeSq, agentLeftOfLine >= 0.0f ? node.right_ : node.left_);
    // //             }
    // //         }
    // //     }

    // private bool queryVisibilityRecursive(Vector2 q1, Vector2 q2, float radius, ObstacleTreeNode node)
    //     {
    //         if (node == null)
    //         {
    //             return true;
    //         }

    //         Obstacle obstacle1 = node.obstacle_;
    //         Obstacle obstacle2 = obstacle1.next_;

    //         float q1LeftOfI = RVOMath.leftOf(obstacle1.point_, obstacle2.point_, q1);
    //         float q2LeftOfI = RVOMath.leftOf(obstacle1.point_, obstacle2.point_, q2);
    //         float invLengthI = 1.0f / RVOMath.absSq(obstacle2.point_ - obstacle1.point_);

    //         if (q1LeftOfI >= 0.0f && q2LeftOfI >= 0.0f)
    //         {
    //             return queryVisibilityRecursive(q1, q2, radius, node.left_) && ((RVOMath.sqr(q1LeftOfI) * invLengthI >= RVOMath.sqr(radius) && RVOMath.sqr(q2LeftOfI) * invLengthI >= RVOMath.sqr(radius)) || queryVisibilityRecursive(q1, q2, radius, node.right_));
    //         }

    //         if (q1LeftOfI <= 0.0f && q2LeftOfI <= 0.0f)
    //         {
    //             return queryVisibilityRecursive(q1, q2, radius, node.right_) && ((RVOMath.sqr(q1LeftOfI) * invLengthI >= RVOMath.sqr(radius) && RVOMath.sqr(q2LeftOfI) * invLengthI >= RVOMath.sqr(radius)) || queryVisibilityRecursive(q1, q2, radius, node.left_));
    //         }

    //         if (q1LeftOfI >= 0.0f && q2LeftOfI <= 0.0f)
    //         {
    //             /* One can see through obstacle from left to right. */
    //             return queryVisibilityRecursive(q1, q2, radius, node.left_) && queryVisibilityRecursive(q1, q2, radius, node.right_);
    //         }

    //         float point1LeftOfQ = RVOMath.leftOf(q1, q2, obstacle1.point_);
    //         float point2LeftOfQ = RVOMath.leftOf(q1, q2, obstacle2.point_);
    //         float invLengthQ = 1.0f / RVOMath.absSq(q2 - q1);

    //         return point1LeftOfQ * point2LeftOfQ >= 0.0f && RVOMath.sqr(point1LeftOfQ) * invLengthQ > RVOMath.sqr(radius) && RVOMath.sqr(point2LeftOfQ) * invLengthQ > RVOMath.sqr(radius) && queryVisibilityRecursive(q1, q2, radius, node.left_) && queryVisibilityRecursive(q1, q2, radius, node.right_);
    //     }

    // internal bool queryVisibility(Vector2 q1, Vector2 q2, float radius)
    //     {
    //         return queryVisibilityRecursive(q1, q2, radius, obstacleTree_);
    //     }

    
}